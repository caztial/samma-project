using Core.Entities.ValueObjects;

namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing emergency contact information.
/// 1:N relationship with UserProfile.
/// Contains Contact value object for contact details.
/// </summary>
public class EmergencyContact : BaseEntity
{
    /// <summary>
    /// Name of the emergency contact person
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Relationship to the user (e.g., "Spouse", "Parent", "Sibling")
    /// </summary>
    public string Relationship { get; set; } = string.Empty;

    /// <summary>
    /// Contact details (phone and email)
    /// </summary>
    public Contact Contact { get; set; } = Contact.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}