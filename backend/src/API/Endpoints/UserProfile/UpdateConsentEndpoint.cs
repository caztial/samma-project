using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update a consent in a profile.
/// Only Admin and Moderator can update consents.
/// </summary>
public class UpdateConsentEndpoint : Endpoint<UpdateConsentRequest, ConsentResponse>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateConsentEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/consents/{consentId}");
        Policy(policy =>
        {
            policy.AddRequirements(new AdminModeratorRequirement());
        });
        Summary(s =>
        {
            s.Summary = "Update consent";
            s.Description = "Updates a consent in a profile. Only Admin or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateConsentRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var consentId = Route<Guid>("consentId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        // Create UserConsent with updated Consent value object
        var userConsent = new UserConsent
        {
            Consent = new Consent(
                req.Consent.TermId,
                req.Consent.TermLink,
                req.Consent.TermsVersion,
                string.Empty // IpAddress is not updated
            )
        };

        var updated = await _userProfileService.UpdateConsentAsync(
            profileId,
            consentId,
            userConsent
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Consent not found" },
                404
            );
            return;
        }

        var response = new ConsentResponse
        {
            Id = updated.Id,
            TermId = updated.Consent.TermId,
            TermLink = updated.Consent.TermLink,
            TermsVersion = updated.Consent.TermsVersion,
            AcceptedAt = updated.Consent.AcceptedAt,
            IpAddress = updated.Consent.IpAddress
        };

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}