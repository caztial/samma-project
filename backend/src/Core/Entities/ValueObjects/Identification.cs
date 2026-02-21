using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing identification numbers (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public sealed class Identification : ValueObject
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Type of identification (e.g., "Passport", "SSN", "DriverLicense").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The identification number/value (encrypted).
    /// </summary>
    [Encrypt]
    public string Value { get; set; } = string.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public Identification() { }

    public Identification(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public static Identification Empty => new(string.Empty, string.Empty);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id is excluded from equality - value objects compare by value, not identity
        yield return Type;
        yield return Value;
    }
}
