using API.DTOs.UserProfile;
using API.Mappers;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an address to a profile.
/// </summary>
public class AddAddressEndpoint
    : Endpoint<(Guid Id, AddressRequest Request), AddressResponse, AddressMapper>
{
    private readonly IUserProfileService _userProfileService;

    public AddAddressEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/api/profile/{id}/addresses");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Add address";
            s.Description = "Adds an address to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, AddressRequest Request) req,
        CancellationToken ct
    )
    {
        var (id, request) = req;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var address = Map.ToEntity(request);

        var added = await _userProfileService.AddAddressAsync(id, address);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Failed to add address" }, 500);
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201);
    }
}
