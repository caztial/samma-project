using API.DTOs.UserProfile;
using API.Mappers;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an identification to a profile.
/// </summary>
public class AddIdentificationEndpoint
    : Endpoint<
        (Guid Id, IdentificationRequest Request),
        IdentificationResponse,
        IdentificationMapper
    >
{
    private readonly IUserProfileService _userProfileService;

    public AddIdentificationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/api/profile/{id}/identifications");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Add identification";
            s.Description =
                "Adds an identification to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, IdentificationRequest Request) ctx,
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

        var identification = Map.ToEntity(req);

        var added = await _userProfileService.AddIdentificationAsync(id, identification);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add identification" },
                500
            );
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201);
    }
}
