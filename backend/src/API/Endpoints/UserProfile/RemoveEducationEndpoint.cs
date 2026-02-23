using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an education from a profile.
/// </summary>
public class RemoveEducationEndpoint : Endpoint<RemoveEducationRequest>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveEducationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/educations/{educationId}");
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
            s.Summary = "Remove education";
            s.Description =
                "Removes an education from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(RemoveEducationRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var educationId = Route<Guid>("educationId");

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var removed = await _userProfileService.RemoveEducationAsync(id, educationId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(new { error = "Education not found" }, 404);
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Education removed successfully" },
            200
        );
    }
}
