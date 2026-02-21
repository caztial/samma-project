using Core.Entities;

namespace Core.Entities.ValueObjects;

/// <summary>
/// Value object representing biometric data (PII - encrypted, stored as Base64).
/// 1:1 relationship with UserProfile.
/// </summary>
public sealed class Biometrics : ValueObject
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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FingerPrint;
        yield return Face;
    }
}
