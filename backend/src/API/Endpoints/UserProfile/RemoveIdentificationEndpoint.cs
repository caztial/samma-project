using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an identification from a profile.
/// </summary>
public class RemoveIdentificationEndpoint : Endpoint<(Guid Id, Guid IdentificationId)>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveIdentificationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/api/profile/{id}/identifications/{identificationId}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Remove identification";
            s.Description =
                "Removes an identification from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, Guid IdentificationId) ctx,
        CancellationToken ct
    )
    {
        var (id, identificationId) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var removed = await _userProfileService.RemoveIdentificationAsync(id, identificationId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(new { error = "Identification not found" }, 404);
            return;
        }

        await HttpContext.Response.SendAsync(new { message = "Identification removed" }, 200);
    }
}
