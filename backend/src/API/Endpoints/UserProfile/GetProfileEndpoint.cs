using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get a user profile by ID.
/// </summary>
public class GetProfileEndpoint
    : Endpoint<GetProfileEndpointRequest, ProfileResponse, ProfileMapper>
{
    private readonly IUserProfileService _userProfileService;

    public GetProfileEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id?}");
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
            s.Summary = "Get user profile";
            s.Description =
                "Retrieves a user profile by ID including all collections. Owner, Admin, or Moderator can access. If ID is not provided, returns the current user's profile.";
        });
    }

    public override async Task HandleAsync(GetProfileEndpointRequest request, CancellationToken ct)
    {
        Guid profileId;

        // If ID is provided in route, use it; otherwise fetch from claims
        if (request.Id.HasValue)
        {
            profileId = request.Id.Value;
        }
        else
        {
            // Get UserId from claims using FromClaim and fetch ProfileId
            var userId = request.UserId;

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

            var profile = await _userProfileService.GetByUserIdAsync(userId);
            if (profile == null)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "Profile not found" },
                    404,
                    cancellation: ct
                );
                return;
            }

            profileId = profile.Id;
        }

        var profileById = await _userProfileService.GetByIdAsync(profileId);

        if (profileById == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Profile not found" },
                404,
                cancellation: ct
            );
            return;
        }

        // Use mapper to convert entity to response (includes all collections)
        Response = Map.FromEntity(profileById);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}

public class GetProfileEndpointRequest
{
    /// <summary>
    /// User ID from JWT claims. Used when profile ID is not provided in route.
    /// </summary>
    [FromClaim("UserId")]
    public string? UserId { get; set; }

    public Guid? Id { get; set; }
}
