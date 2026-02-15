using API.DTOs.UserProfile;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add a consent to a profile.
/// </summary>
public class AddConsentEndpoint : Endpoint<(Guid Id, ConsentRequest Request), ConsentResponse>
{
    private readonly IUserProfileService _userProfileService;

    public AddConsentEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/api/profile/{id}/consents");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Add consent";
            s.Description = "Adds a consent to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, ConsentRequest Request) ctx,
        CancellationToken ct
    )
    {
        var (id, req) = ctx;

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        // Get client IP address
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Create consent directly since it needs IP address at creation time
        var consent = new Consent(req.TermId, req.TermLink, req.TermsVersion, ipAddress);

        var added = await _userProfileService.AddConsentAsync(id, consent);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Failed to add consent" }, 500);
            return;
        }

        var response = new ConsentResponse
        {
            Id = added.Id,
            TermId = added.TermId,
            TermLink = added.TermLink,
            TermsVersion = added.TermsVersion,
            AcceptedAt = added.AcceptedAt,
            IpAddress = added.IpAddress
        };

        await HttpContext.Response.SendAsync(response, 201);
    }
}
