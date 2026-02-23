namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating an education.
/// </summary>
public class UpdateEducationRequest
{
    public EducationRequest Education { get; set; } = new();
}

/// <summary>
/// Request for updating education route parameters.
/// </summary>
public class UpdateEducationEndpointRequest
{
    public Guid Id { get; set; }
    public Guid EducationId { get; set; }
}
