using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an address from a profile.
/// </summary>
public class RemoveAddressEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public RemoveAddressEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/addresses/{addressId}");
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
            s.Summary = "Remove address";
            s.Description =
                "Removes an address from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var addressId = Route<Guid>("addressId");

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

        var removed = await _userProfileService.RemoveAddressAsync(id, addressId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Address not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Address removed" },
            200,
            cancellation: ct
        );
    }
}
