namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an identification.
/// </summary>
public class IdentificationRequest
{
    public string? CIN { get; set; }
    public string? PassportNumber { get; set; }
}

/// <summary>
/// Response DTO for identification.
/// </summary>
public class IdentificationResponse
{
    public Guid Id { get; set; }
    public string? CIN { get; set; }
    public string? PassportNumber { get; set; }
}
