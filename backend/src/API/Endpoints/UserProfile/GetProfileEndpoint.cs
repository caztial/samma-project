using API.DTOs.UserProfile;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get a user profile by ID.
/// </summary>
public class GetProfileEndpoint : Endpoint<Guid, ProfileResponse, ProfileMapper>
{
    private readonly IUserProfileService _userProfileService;

    public GetProfileEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/api/profile/{id}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Get user profile";
            s.Description =
                "Retrieves a user profile by ID including all collections. Owner, Admin, or Moderator can access.";
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

        // Use mapper to convert entity to response (includes all collections)
        Response = Map.FromEntity(profile);
        await HttpContext.Response.SendAsync(Response, 200);
    }
}
