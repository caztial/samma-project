namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating a consent.
/// </summary>
public class UpdateConsentRequest
{
    public ConsentRequest Consent { get; set; } = new();
}

/// <summary>
/// Request for updating consent route parameters.
/// </summary>
public class UpdateConsentEndpointRequest
{
    public Guid Id { get; set; }
    public Guid ConsentId { get; set; }
}
