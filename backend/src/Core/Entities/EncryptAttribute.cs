namespace Core.Entities;

/// <summary>
/// Attribute to mark properties that should be encrypted using EF Core Value Converters.
/// Apply this attribute to string properties that contain PII data.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EncryptAttribute : Attribute { }
