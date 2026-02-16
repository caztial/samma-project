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
        Get("/profile/{id}");
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
            s.Summary = "Get user profile";
            s.Description =
                "Retrieves a user profile by ID including all collections. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(GetProfileEndpointRequest request, CancellationToken ct)
    {
        var profile = await _userProfileService.GetByIdAsync(request.Id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Profile not found" },
                404,
                cancellation: ct
            );
            return;
        }

        // Use mapper to convert entity to response (includes all collections)
        Response = Map.FromEntity(profile);
        await HttpContext.Response.SendAsync(Response, 200, cancellation: ct);
    }
}

public class GetProfileEndpointRequest
{
    public Guid Id { get; set; }
}
