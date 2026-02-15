namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating biometrics.
/// </summary>
public class BiometricsRequest
{
    public string? FingerPrint { get; set; }
    public string? Face { get; set; }
}

/// <summary>
/// Response DTO for biometrics.
/// </summary>
public class BiometricsResponse
{
    public string? FingerPrint { get; set; }
    public string? Face { get; set; }
}
