using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an address to a profile.
/// </summary>
public class AddAddressEndpoint : Endpoint<AddAddressRequest, AddressResponse>
{
    private readonly IUserProfileService _userProfileService;

    public AddAddressEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/profile/{id}/addresses");
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
            s.Summary = "Add address";
            s.Description = "Adds an address to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(AddAddressRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var userAddress = new UserAddress
        {
            Type = req.Type,
            Address = new Core.Entities.ValueObjects.Address(
                req.Address.Line1,
                req.Address.Suburb,
                req.Address.StateProvince,
                req.Address.Country,
                req.Address.Postcode,
                req.Address.Line2
            )
        };

        var added = await _userProfileService.AddAddressAsync(id, userAddress);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add address" },
                500,
                cancellation: ct
            );
            return;
        }

        Response = new AddressResponse
        {
            Id = added.Id,
            Type = added.Type,
            Line1 = added.Address.Line1,
            Line2 = added.Address.Line2,
            Suburb = added.Address.Suburb,
            StateProvince = added.Address.StateProvince,
            Country = added.Address.Country,
            Postcode = added.Address.Postcode
        };
        await HttpContext.Response.SendAsync(Response, 201);
    }
}