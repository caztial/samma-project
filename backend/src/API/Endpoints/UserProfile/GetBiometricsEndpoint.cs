using API.DTOs.UserProfile;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get biometrics for a profile.
/// </summary>
public class GetBiometricsEndpoint : Endpoint<Guid, BiometricsResponse>
{
    private readonly IUserProfileService _userProfileService;

    public GetBiometricsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/api/profile/{id}/biometrics");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Get biometrics";
            s.Description =
                "Retrieves biometrics for a profile. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(Guid id, CancellationToken ct)
    {
        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var response = new BiometricsResponse
        {
            FingerPrint = profile.Biometrics.FingerPrint,
            Face = profile.Biometrics.Face
        };

        await HttpContext.Response.SendAsync(response, 200);
    }
}
