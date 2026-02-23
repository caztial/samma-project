namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for removing an education.
/// </summary>
public class RemoveEducationRequest
{
    public Guid Id { get; set; }
    public Guid EducationId { get; set; }
}
