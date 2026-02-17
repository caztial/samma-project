using Core.Enums;

namespace API.DTOs.UserProfile;

/// <summary>
/// Response DTO for getting user profile - includes full aggregate.
/// </summary>
public class ProfileResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    // Primary contact
    public ContactDto Contact { get; set; } = new();

    // Biometrics
    public BiometricsDto Biometrics { get; set; } = new();

    // Collections
    public List<EmergencyContactResponse> EmergencyContacts { get; set; } = [];
    public List<AddressResponse> Addresses { get; set; } = [];
    public List<IdentificationResponse> Identifications { get; set; } = [];
    public List<ConsentResponse> Consents { get; set; } = [];
}

public class ContactDto
{
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class BiometricsDto
{
    public string? FingerPrint { get; set; }
    public string? Face { get; set; }
}
