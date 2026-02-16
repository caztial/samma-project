using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an emergency contact from a profile.
/// </summary>
public class RemoveEmergencyContactEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public RemoveEmergencyContactEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/emergency-contacts/{emergencyContactId}");
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
            s.Summary = "Remove emergency contact";
            s.Description =
                "Removes an emergency contact from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var emergencyContactId = Route<Guid>("emergencyContactId");

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

        var removed = await _userProfileService.RemoveEmergencyContactAsync(id, emergencyContactId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Emergency contact not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Emergency contact removed" },
            200,
            cancellation: ct
        );
    }
}
