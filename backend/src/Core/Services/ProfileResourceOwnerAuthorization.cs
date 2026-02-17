using System.Security.Claims;
using Core.Enums;

namespace Core.Services;

/// <summary>
/// Resource owner authorization implementation for UserProfile.
/// Checks if the user has Admin/Moderator role first, then checks if user is the owner.
/// </summary>
public class ProfileResourceOwnerAuthorization : IResourceOwnerAuthorization
{
    private readonly IUserProfileService _userProfileService;

    public ProfileResourceOwnerAuthorization(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    /// <inheritdoc/>
    public async Task<bool> IsOwnerAsync(Guid id, ClaimsPrincipal claims)
    {
        // Check role first - Admin and Moderator have full access
        if (HasElevatedRole(claims))
        {
            return true;
        }

        // Get the profile
        var profile = await _userProfileService.GetByIdAsync(id);
        if (profile == null)
        {
            // Profile not found - return false (not authorized)
            return false;
        }

        // Get current user's ID from claims
        var userIdClaim = claims.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return false;
        }

        // Check if user is owner
        return userIdClaim == profile.UserId;
    }

    private static bool HasElevatedRole(ClaimsPrincipal claims)
    {
        return claims.IsInRole(ApplicationRoles.Admin.ToValueString())
            || claims.IsInRole(ApplicationRoles.Moderator.ToValueString());
    }
}
