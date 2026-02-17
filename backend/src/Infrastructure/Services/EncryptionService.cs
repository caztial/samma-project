using System.Security.Cryptography;
using System.Text;
using Core.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of IEncryptionService using AES with key and IV from configuration.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        var key =
            configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption:Key is not configured");
        var iv =
            configuration["Encryption:IV"]
            ?? throw new InvalidOperationException("Encryption:IV is not configured");

        _key = Convert.FromBase64String(key);
        _iv = Convert.FromBase64String(iv);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        try
        {
            var buffer = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }
        catch (Exception)
        {
            // Return empty if decryption fails (e.g., data was not encrypted)
            return string.Empty;
        }
    }
}
