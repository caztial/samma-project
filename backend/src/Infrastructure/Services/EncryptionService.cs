using Core.Services;
using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of IEncryptionService using ASP.NET Core Data Protection (AES).
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public EncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("UserProfile.PII.v1");
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        return _protector.Protect(plainText);
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        try
        {
            return _protector.Unprotect(encryptedText);
        }
        catch (Exception)
        {
            // Return empty if decryption fails (e.g., data was not encrypted)
            return string.Empty;
        }
    }
}
