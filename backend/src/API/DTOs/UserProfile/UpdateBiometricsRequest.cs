namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating biometrics in a profile.
/// </summary>
public class UpdateBiometricsRequest
{
    public BiometricsRequest Biometrics { get; set; } = new();
}
