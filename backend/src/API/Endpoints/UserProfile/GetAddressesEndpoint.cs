using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all addresses for a profile.
/// </summary>
public class GetAddressesEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetAddressesEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/addresses");
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
            s.Summary = "Get addresses";
            s.Description =
                "Retrieves all addresses for a profile. Owner, Admin, or Moderator can access.";
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

        var addresses = await _userProfileService.GetAddressesAsync(id);

        var response = addresses.Select(a => new AddressResponse
        {
            Id = a.Id,
            Line1 = a.Line1,
            Line2 = a.Line2,
            Suburb = a.Suburb,
            StateProvince = a.StateProvince,
            Country = a.Country,
            Postcode = a.Postcode
        });

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
