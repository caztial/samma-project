using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an address from a profile.
/// </summary>
public class RemoveAddressEndpoint : Endpoint<(Guid Id, Guid AddressId)>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveAddressEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/api/profile/{id}/addresses/{addressId}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Remove address";
            s.Description =
                "Removes an address from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync((Guid Id, Guid AddressId) ctx, CancellationToken ct)
    {
        var (id, addressId) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var removed = await _userProfileService.RemoveAddressAsync(id, addressId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(new { error = "Address not found" }, 404);
            return;
        }

        await HttpContext.Response.SendAsync(new { message = "Address removed" }, 200);
    }
}
