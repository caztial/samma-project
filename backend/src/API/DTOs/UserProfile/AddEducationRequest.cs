namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an education to a profile.
/// </summary>
public class AddEducationRequest
{
    public EducationRequest Education { get; set; } = new();
}
