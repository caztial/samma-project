using API.DTOs.UserProfile;
using API.Mappers;
using Core.Entities.ValueObjects;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update a user profile.
/// </summary>
public class UpdateProfileEndpoint : Endpoint<UpdateProfileRequest, ProfileResponse, ProfileMapper>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateProfileEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/api/profile/{id}");
        Roles("Admin", "Moderator");
        Policies("ProfileOwner");
        Summary(s =>
        {
            s.Summary = "Update user profile";
            s.Description = "Updates a user profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateProfileRequest req, CancellationToken ct)
    {
        // Get profile ID from route
        var profileId = Route<Guid>("id");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        // Convert Contact DTO to ValueObject
        Contact? contact = null;
        if (req.Contact != null)
        {
            contact = new Contact(
                req.Contact.ContactNumber ?? string.Empty,
                req.Contact.Email ?? string.Empty
            );
        }

        // Update profile
        var updated = await _userProfileService.UpdateProfileAsync(
            profileId,
            req.FirstName,
            req.LastName,
            req.ProfileImageUrl,
            req.Gender,
            req.DateOfBirth,
            contact
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Failed to update profile" }, 500);
            return;
        }

        // Use mapper for response mapping
        Response = Map.FromEntity(updated);
        await HttpContext.Response.SendAsync(Response, 200);
    }
}
