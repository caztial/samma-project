namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating an emergency contact.
/// </summary>
public class UpdateEmergencyContactRequest
{
    public EmergencyContactRequest EmergencyContact { get; set; } = new();
}

/// <summary>
/// Request for updating emergency contact route parameters.
/// </summary>
public class UpdateEmergencyContactEndpointRequest
{
    public Guid Id { get; set; }
    public Guid EmergencyContactId { get; set; }
}
