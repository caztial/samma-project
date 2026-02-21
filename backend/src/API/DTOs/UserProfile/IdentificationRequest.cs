namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for adding an identification.
/// </summary>
public class IdentificationRequest
{
    /// <summary>
    /// Type of identification (e.g., "Passport", "SSN", "DriverLicense").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The identification number/value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for identification.
/// </summary>
public class IdentificationResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
