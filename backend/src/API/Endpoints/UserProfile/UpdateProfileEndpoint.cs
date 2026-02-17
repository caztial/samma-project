using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update a user profile.
/// </summary>
public class UpdateProfileEndpoint : Endpoint<UpdateProfileRequest, ProfileResponse, ProfileMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateProfileEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id?}");
        Policy(policy =>
        {
            policy.AddRequirements(
                new AdminOwnerRequirement(
                    aggregatedRootName: nameof(UserProfile),
                    resourceIdParameterName: "id",
                    valueFetchFrom: ValueFetchFrom.Route,
                    valueNullable: true
                )
            );
        });
        Summary(s =>
        {
            s.Summary = "Update user profile";
            s.Description =
                "Updates a user profile. Owner, Admin, or Moderator can update. If ID is not provided, updates the current user's profile.";
        });
    }

    public override async Task HandleAsync(UpdateProfileRequest req, CancellationToken ct)
    {
        Guid profileId;

        // Get profile ID from route, otherwise derive from claims
        if (Route<Guid?>("id").HasValue)
        {
            profileId = Route<Guid>("id");
        }
        else
        {
            // Get UserId from claims using FromClaim and fetch ProfileId
            var userId = req.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                await HttpContext.Response.SendAsync(
                    new
                    {
                        error = "Unable to identify user. Please provide a profile ID or ensure you are authenticated."
                    },
                    401,
                    cancellation: ct
                );
                return;
            }

            var profileByUserId = await _userProfileService.GetByUserIdAsync(userId);
            if (profileByUserId == null)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "Profile not found" },
                    404,
                    cancellation: ct
                );
                return;
            }

            profileId = profileByUserId.Id;
        }

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        // Parse Gender string to enum
        Gender? gender = null;
        if (!string.IsNullOrEmpty(req.Gender))
        {
            gender = Enum.Parse<Gender>(req.Gender, ignoreCase: true);
        }

        // Parse DateOfBirth string to DateOnly
        DateOnly? dateOfBirth = null;
        if (!string.IsNullOrEmpty(req.DateOfBirth))
        {
            dateOfBirth = DateOnly.Parse(req.DateOfBirth);
        }

        // Convert flattened contact fields to ValueObject
        Contact? contact = null;
        if (!string.IsNullOrEmpty(req.ContactNumber) || !string.IsNullOrEmpty(req.Email))
        {
            contact = new Contact(req.ContactNumber ?? string.Empty, req.Email ?? string.Empty);
        }

        // Update profile
        var updated = await _userProfileService.UpdateProfileAsync(
            profileId,
            req.FirstName,
            req.LastName,
            req.ProfileImageUrl,
            gender,
            dateOfBirth,
            contact
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Failed to update profile" }, 500);
            return;
        }

        // Use mapper for response mapping
        Response = Map.FromEntity(updated);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}
