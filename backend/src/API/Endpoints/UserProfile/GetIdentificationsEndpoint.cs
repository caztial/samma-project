using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all identifications for a profile.
/// </summary>
public class GetIdentificationsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetIdentificationsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/identifications");
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
            s.Summary = "Get identifications";
            s.Description =
                "Retrieves all identifications for a profile. Owner, Admin, or Moderator can access.";
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

        var identifications = await _userProfileService.GetIdentificationsAsync(id);

        var response = identifications.Select(i => new IdentificationResponse
        {
            Id = i.Id,
            CIN = i.CIN,
            PassportNumber = i.PassportNumber
        });

        await HttpContext.Response.SendAsync(response, 200);
    }
}
