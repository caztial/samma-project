namespace Core.Services;

/// <summary>
/// Interface for encryption/decryption service using AES via IDataProtector.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a string value.
    /// </summary>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts an encrypted string value.
    /// </summary>
    string Decrypt(string encryptedText);
}
