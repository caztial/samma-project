using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an identification from a profile.
/// </summary>
public class RemoveIdentificationEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public RemoveIdentificationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/identifications/{identificationId}");
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
            s.Summary = "Remove identification";
            s.Description =
                "Removes an identification from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var identificationId = Route<Guid>("identificationId");

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

        var removed = await _userProfileService.RemoveIdentificationAsync(id, identificationId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Identification not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Identification removed" },
            200,
            cancellation: ct
        );
    }
}
