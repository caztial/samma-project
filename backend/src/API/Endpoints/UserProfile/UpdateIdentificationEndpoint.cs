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
/// Endpoint to update an identification in a profile.
/// </summary>
public class UpdateIdentificationEndpoint
    : Endpoint<UpdateIdentificationRequest, IdentificationResponse, IdentificationMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateIdentificationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/identifications/{identificationId}");
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
            s.Summary = "Update identification";
            s.Description =
                "Updates an identification in a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateIdentificationRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var identificationId = Route<Guid>("identificationId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var identification = Map.ToEntity(req.Identification);

        var updated = await _userProfileService.UpdateIdentificationAsync(
            profileId,
            identificationId,
            identification
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Identification not found" },
                404
            );
            return;
        }

        Response = Map.FromEntity(updated);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}
