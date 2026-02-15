using System.Reflection;
using Core.Entities;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Encryption;

/// <summary>
/// EF Core Value Converter for automatic encryption/decryption of string PII fields.
/// </summary>
public class EncryptionConverter : ValueConverter<string, string>
{
    public EncryptionConverter(IEncryptionService encryptionService)
        : base(
            // Convert to database (encrypt)
            v => encryptionService.Encrypt(v),
            // Convert from database (decrypt)
            v => encryptionService.Decrypt(v)
        ) { }
}

/// <summary>
/// Extension methods to apply encryption converters to properties marked with [Encrypt].
/// </summary>
public static class EncryptionExtensions
{
    /// <summary>
    /// Applies encryption converters to all string properties marked with [Encrypt].
    /// </summary>
    public static void ApplyEncryption(
        this ModelBuilder modelBuilder,
        IEncryptionService encryptionService
    )
    {
        var converter = new EncryptionConverter(encryptionService);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var entityTypeclr = entityType.ClrType;

            foreach (var property in entityType.GetProperties())
            {
                if (
                    property.ClrType == typeof(string)
                    && entityTypeclr
                        .GetProperty(property.Name)
                        ?.GetCustomAttribute<EncryptAttribute>() != null
                )
                {
                    property.SetValueConverter(converter);
                }
            }
        }
    }
}
