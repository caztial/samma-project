using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing identification numbers (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public sealed class Identification
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Encrypt]
    public string? CIN { get; set; } // Country specific Identification Number

    [Encrypt]
    public string? PassportNumber { get; set; }

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public Identification() { }

    public Identification(string? cin, string? passportNumber)
    {
        CIN = cin;
        PassportNumber = passportNumber;
    }

    public static Identification Empty => new(null, null);

    public override bool Equals(object? obj)
    {
        if (obj is not Identification other)
            return false;
        return CIN == other.CIN && PassportNumber == other.PassportNumber;
    }

    public override int GetHashCode() => HashCode.Combine(CIN, PassportNumber);

    public static bool operator ==(Identification? left, Identification? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Identification? left, Identification? right) => !(left == right);
}
