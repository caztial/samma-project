using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove a consent from a profile.
/// </summary>
public class RemoveConsentEndpoint : Endpoint<(Guid Id, Guid ConsentId)>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveConsentEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/api/profile/{id}/consents/{consentId}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Remove consent";
            s.Description =
                "Removes a consent from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync((Guid Id, Guid ConsentId) ctx, CancellationToken ct)
    {
        var (id, consentId) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var removed = await _userProfileService.RemoveConsentAsync(id, consentId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(new { error = "Consent not found" }, 404);
            return;
        }

        await HttpContext.Response.SendAsync(new { message = "Consent removed" }, 200);
    }
}
