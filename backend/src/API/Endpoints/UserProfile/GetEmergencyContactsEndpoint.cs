using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all emergency contacts for a profile.
/// </summary>
public class GetEmergencyContactsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetEmergencyContactsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/emergency-contacts");
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
            s.Summary = "Get emergency contacts";
            s.Description =
                "Retrieves all emergency contacts for a profile. Owner, Admin, or Moderator can access.";
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

        var contacts = await _userProfileService.GetEmergencyContactsAsync(id);

        var response = contacts.Select(ec => new EmergencyContactResponse
        {
            Id = ec.Id,
            Name = ec.Name,
            ContactNumber = ec.ContactNumber,
            Relationship = ec.Relationship,
            Email = ec.Email
        });

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
