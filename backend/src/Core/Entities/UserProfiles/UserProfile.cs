using Core.Entities;
using Core.Entities.ValueObjects;
using Core.Enums;

namespace Core.Entities.UserProfiles;

/// <summary>
/// UserProfile Aggregate Root - contains all user profile information including PII data.
/// Created via UserCreatedEvent.
/// </summary>
public class UserProfile : BaseEntity, IAggregatedRoot
{
    /// <summary>
    /// Links to ApplicationUser (IdentityUser)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// First name (PII - encrypted)
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (PII - encrypted)
    /// </summary>
    [Encrypt]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Profile image URL
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Date of birth
    /// </summary>
    [Encrypt]
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Primary contact information (PII - encrypted)
    /// </summary>
    public Contact Contact { get; set; } = Contact.Empty;

    /// <summary>
    /// Emergency contacts (PII - encrypted) - 1:N
    /// </summary>
    public ICollection<EmergencyContact> EmergencyContacts { get; set; } =
        new List<EmergencyContact>();

    /// <summary>
    /// Addresses - 1:N
    /// </summary>
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    /// <summary>
    /// Identification numbers (PII - encrypted) - 1:N
    /// </summary>
    public ICollection<Identification> Identifications { get; set; } = new List<Identification>();

    /// <summary>
    /// Biometrics data (PII - encrypted, Base64) - 1:1
    /// </summary>
    public Biometrics Biometrics { get; set; } = Biometrics.Empty;

    /// <summary>
    /// Consent records - 1:N
    /// </summary>
    public ICollection<Consent> Consents { get; set; } = new List<Consent>();

    /// <summary>
    /// Factory method to create UserProfile from UserCreatedEvent
    /// </summary>
    public static UserProfile CreateFromEvent(Events.UserCreatedEvent userEvent)
    {
        return new UserProfile
        {
            UserId = userEvent.UserId,
            FirstName = userEvent.FirstName,
            LastName = userEvent.LastName,
            Contact = new Contact(string.Empty, userEvent.Email),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Full name convenience property
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
