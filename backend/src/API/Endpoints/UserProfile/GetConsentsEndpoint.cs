using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all consents for a profile.
/// </summary>
public class GetConsentsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetConsentsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/consents");
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
            s.Summary = "Get consents";
            s.Description =
                "Retrieves all consents for a profile. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Profile not found" },
                404,
                cancellation: ct
            );
            return;
        }

        var consents = await _userProfileService.GetConsentsAsync(id);

        var response = consents.Select(c => new ConsentResponse
        {
            Id = c.Id,
            TermId = c.Consent.TermId,
            TermLink = c.Consent.TermLink,
            TermsVersion = c.Consent.TermsVersion,
            AcceptedAt = c.Consent.AcceptedAt,
            IpAddress = c.Consent.IpAddress
        });

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}