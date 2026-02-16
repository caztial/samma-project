using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an identification to a profile.
/// </summary>
public class AddIdentificationEndpoint
    : Endpoint<AddIdentificationRequest, IdentificationResponse, IdentificationMapper>
{
    private readonly IUserProfileService _userProfileService;

    public AddIdentificationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/profile/{id}/identifications");
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
            s.Summary = "Add identification";
            s.Description =
                "Adds an identification to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(AddIdentificationRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var identification = Map.ToEntity(req.Identification);

        var added = await _userProfileService.AddIdentificationAsync(id, identification);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add identification" },
                500
            );
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
    }
}
