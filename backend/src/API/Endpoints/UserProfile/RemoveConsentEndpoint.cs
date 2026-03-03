using Core.Authorization;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove a consent from a profile.
/// Only Admin and Moderator can remove consents.
/// </summary>
public class RemoveConsentEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public RemoveConsentEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/consents/{consentId}");
        Policy(policy =>
        {
            policy.AddRequirements(new AdminModeratorRequirement());
        });
        Summary(s =>
        {
            s.Summary = "Remove consent";
            s.Description = "Removes a consent from a profile. Only Admin or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var consentId = Route<Guid>("consentId");

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
