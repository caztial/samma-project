using Core.Entities;

namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing identification numbers (PII - encrypted).
/// 1:N relationship with UserProfile.
/// </summary>
public class Identification : BaseEntity
{
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
    public UserProfile? UserProfile { get; set; }
}