using API.DTOs.UserProfile;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all identifications for a profile.
/// </summary>
public class GetIdentificationsEndpoint : Endpoint<Guid, IEnumerable<IdentificationResponse>>
{
    private readonly IUserProfileService _userProfileService;

    public GetIdentificationsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/api/profile/{id}/identifications");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Get identifications";
            s.Description =
                "Retrieves all identifications for a profile. Owner, Admin, or Moderator can access.";
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

        var identifications = await _userProfileService.GetIdentificationsAsync(id);

        var response = identifications.Select(i => new IdentificationResponse
        {
            Id = i.Id,
            CIN = i.CIN,
            PassportNumber = i.PassportNumber
        });

        await HttpContext.Response.SendAsync(response, 200);
    }
}
