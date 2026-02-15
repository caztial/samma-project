using Core.Enums;

namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating user profile.
/// </summary>
public class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public ContactDto? Contact { get; set; }
}

public class UpdateProfileRequestContact
{
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
}
