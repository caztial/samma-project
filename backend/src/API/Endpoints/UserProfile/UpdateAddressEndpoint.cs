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
/// Endpoint to update an address in a profile.
/// </summary>
public class UpdateAddressEndpoint : Endpoint<UpdateAddressRequest, AddressResponse, AddressMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateAddressEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/addresses/{addressId}");
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
            s.Summary = "Update address";
            s.Description =
                "Updates an address in a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateAddressRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var addressId = Route<Guid>("addressId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var address = Map.ToEntity(req.Address);

        var updated = await _userProfileService.UpdateAddressAsync(profileId, addressId, address);

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Address not found" }, 404);
            return;
        }

        Response = Map.FromEntity(updated);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}
