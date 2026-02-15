using API.DTOs.UserProfile;
using API.Mappers;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update biometrics for a profile.
/// </summary>
public class UpdateBiometricsEndpoint
    : Endpoint<(Guid Id, BiometricsRequest Request), BiometricsResponse, BiometricsMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateBiometricsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/api/profile/{id}/biometrics");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Update biometrics";
            s.Description =
                "Updates biometrics for a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, BiometricsRequest Request) ctx,
        CancellationToken ct
    )
    {
        var (id, req) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var biometrics = Map.ToEntity(req);

        var updated = await _userProfileService.UpdateBiometricsAsync(id, biometrics);

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to update biometrics" },
                500
            );
            return;
        }

        Response = Map.FromEntity(updated.Biometrics);
        await HttpContext.Response.SendAsync(Response, 200);
    }
}
