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
/// Endpoint to update an emergency contact in a profile.
/// </summary>
public class UpdateEmergencyContactEndpoint
    : Endpoint<UpdateEmergencyContactRequest, EmergencyContactResponse, EmergencyContactMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateEmergencyContactEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/emergency-contacts/{emergencyContactId}");
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
            s.Summary = "Update emergency contact";
            s.Description =
                "Updates an emergency contact in a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateEmergencyContactRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var emergencyContactId = Route<Guid>("emergencyContactId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var emergencyContact = Map.ToEntity(req.EmergencyContact);

        var updated = await _userProfileService.UpdateEmergencyContactAsync(
            profileId,
            emergencyContactId,
            emergencyContact
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Emergency contact not found" },
                404
            );
            return;
        }

        Response = Map.FromEntity(updated);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}
