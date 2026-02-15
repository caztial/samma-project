namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an emergency contact.
/// </summary>
public class EmergencyContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for emergency contact.
/// </summary>
public class EmergencyContactResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
