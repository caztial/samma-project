using API.DTOs.UserProfile;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all consents for a profile.
/// </summary>
public class GetConsentsEndpoint : Endpoint<Guid, IEnumerable<ConsentResponse>>
{
    private readonly IUserProfileService _userProfileService;

    public GetConsentsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/api/profile/{id}/consents");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Get consents";
            s.Description =
                "Retrieves all consents for a profile. Owner, Admin, or Moderator can access.";
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

        var consents = await _userProfileService.GetConsentsAsync(id);

        var response = consents.Select(c => new ConsentResponse
        {
            Id = c.Id,
            TermId = c.TermId,
            TermLink = c.TermLink,
            TermsVersion = c.TermsVersion,
            AcceptedAt = c.AcceptedAt,
            IpAddress = c.IpAddress
        });

        await HttpContext.Response.SendAsync(response, 200);
    }
}
