using API.DTOs.UserProfile;
using API.Mappers;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an emergency contact to a profile.
/// </summary>
public class AddEmergencyContactEndpoint
    : Endpoint<
        (Guid Id, EmergencyContactRequest Request),
        EmergencyContactResponse,
        EmergencyContactMapper
    >
{
    private readonly IUserProfileService _userProfileService;

    public AddEmergencyContactEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/api/profile/{id}/emergency-contacts");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Add emergency contact";
            s.Description =
                "Adds an emergency contact to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(
        (Guid Id, EmergencyContactRequest Request) ctx,
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

        var emergencyContact = Map.ToEntity(ctx.Request);

        var added = await _userProfileService.AddEmergencyContactAsync(id, emergencyContact);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add emergency contact" },
                500
            );
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201);
    }
}
