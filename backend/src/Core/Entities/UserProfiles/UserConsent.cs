using Core.Entities.ValueObjects;

namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing a user's consent record - 1:N relationship with UserProfile.
/// Contains Consent value object.
/// </summary>
public class UserConsent : BaseEntity
{
    /// <summary>
    /// The consent details (value object)
    /// </summary>
    public Consent Consent { get; set; } = Consent.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}