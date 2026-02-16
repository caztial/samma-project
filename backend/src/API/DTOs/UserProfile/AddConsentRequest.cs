namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding a consent to a profile.
/// </summary>
public class AddConsentRequest
{
    public ConsentRequest Consent { get; set; } = new();
}
