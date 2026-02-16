namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an emergency contact to a profile.
/// </summary>
public class AddEmergencyContactRequest
{
    public EmergencyContactRequest EmergencyContact { get; set; } = new();
}
