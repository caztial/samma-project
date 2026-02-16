namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an identification to a profile.
/// </summary>
public class AddIdentificationRequest
{
    public IdentificationRequest Identification { get; set; } = new();
}
