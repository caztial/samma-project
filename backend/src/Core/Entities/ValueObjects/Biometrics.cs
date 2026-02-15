using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing biometric data (PII - encrypted, stored as Base64).
/// 1:1 relationship with UserProfile.
/// </summary>
public sealed class Biometrics
{
    [Encrypt]
    public string? FingerPrint { get; private set; } // Base64 encoded

    [Encrypt]
    public string? Face { get; private set; } // Base64 encoded

    // For EF Core navigation
    public Guid UserProfileId { get; set; }

    public Biometrics() { }

    public Biometrics(string? fingerPrint, string? face)
    {
        FingerPrint = fingerPrint;
        Face = face;
    }

    public static Biometrics Empty => new(null, null);

    public bool HasData => !string.IsNullOrEmpty(FingerPrint) || !string.IsNullOrEmpty(Face);

    public override bool Equals(object? obj)
    {
        if (obj is not Biometrics other)
            return false;
        return FingerPrint == other.FingerPrint && Face == other.Face;
    }

    public override int GetHashCode() => HashCode.Combine(FingerPrint, Face);

    public static bool operator ==(Biometrics? left, Biometrics? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Biometrics? left, Biometrics? right) => !(left == right);
}
