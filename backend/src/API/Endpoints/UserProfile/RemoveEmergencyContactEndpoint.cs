using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove an emergency contact from a profile.
/// </summary>
public class RemoveEmergencyContactEndpoint : Endpoint<(Guid Id, Guid EmergencyContactId)>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveEmergencyContactEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/api/profile/{id}/emergency-contacts/{emergencyContactId}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Remove emergency contact";
            s.Description =
                "Removes an emergency contact from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, Guid EmergencyContactId) ctx,
        CancellationToken ct
    )
    {
        var (id, emergencyContactId) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var removed = await _userProfileService.RemoveEmergencyContactAsync(id, emergencyContactId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Emergency contact not found" },
                404
            );
            return;
        }

        await HttpContext.Response.SendAsync(new { message = "Emergency contact removed" }, 200);
    }
}
