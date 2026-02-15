using API.DTOs.UserProfile;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all addresses for a profile.
/// </summary>
public class GetAddressesEndpoint : Endpoint<Guid, IEnumerable<AddressResponse>>
{
    private readonly IUserProfileService _userProfileService;

    public GetAddressesEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/api/profile/{id}/addresses");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Get addresses";
            s.Description =
                "Retrieves all addresses for a profile. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(Guid id, CancellationToken ct)
    {
        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
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

        await HttpContext.Response.SendAsync(response, 200);
    }
}
