using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get biometrics for a profile.
/// </summary>
public class GetBiometricsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetBiometricsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/biometrics");
        Policy(policy =>
        {
            policy.AddRequirements(
                new AdminOwnerRequirement(
                    aggregatedRootName: nameof(UserProfile),
                    resourceIdParameterName: "id",
                    valueFetchFrom: ValueFetchFrom.Route
                )
            );
        });
        Summary(s =>
        {
            s.Summary = "Get biometrics";
            s.Description =
                "Retrieves biometrics for a profile. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Profile not found" },
                404,
                cancellation: ct
            );
            return;
        }

        var response = new BiometricsResponse
        {
            FingerPrint = profile.Biometrics.FingerPrint,
            Face = profile.Biometrics.Face
        };

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
