using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add a consent to a profile.
/// </summary>
public class AddConsentEndpoint : Endpoint<AddConsentRequest, ConsentResponse>
{
    private readonly IUserProfileService _userProfileService;

    public AddConsentEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/profile/{id}/consents");
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
            s.Summary = "Add consent";
            s.Description = "Adds a consent to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(AddConsentRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        // Get client IP address
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Create UserConsent with nested Consent value object
        var userConsent = new UserConsent
        {
            Consent = new Consent(
                req.Consent.TermId,
                req.Consent.TermLink,
                req.Consent.TermsVersion,
                ipAddress
            )
        };

        var added = await _userProfileService.AddConsentAsync(id, userConsent);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Failed to add consent" }, 500);
            return;
        }

        var response = new ConsentResponse
        {
            Id = added.Id,
            TermId = added.Consent.TermId,
            TermLink = added.Consent.TermLink,
            TermsVersion = added.Consent.TermsVersion,
            AcceptedAt = added.Consent.AcceptedAt,
            IpAddress = added.Consent.IpAddress
        };

        await HttpContext.Response.SendAsync(response, 201, cancellation: ct);
    }
}
