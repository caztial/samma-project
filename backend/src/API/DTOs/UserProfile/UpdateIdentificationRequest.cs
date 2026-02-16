namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating an identification.
/// </summary>
public class UpdateIdentificationRequest
{
    public IdentificationRequest Identification { get; set; } = new();
}

/// <summary>
/// Request for updating identification route parameters.
/// </summary>
public class UpdateIdentificationEndpointRequest
{
    public Guid Id { get; set; }
    public Guid IdentificationId { get; set; }
}
