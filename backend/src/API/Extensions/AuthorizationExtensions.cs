using System.Security.Claims;
using Core.Enums;

namespace API.Extensions;

/// <summary>
/// Extension methods for authorization checks.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Checks if the current user has permission to edit the profile.
    /// </summary>
    /// <param name="claims">The user's claims.</param>
    /// <param name="profileUserId">The user ID of the profile to edit.</param>
    /// <returns>True if the user can edit the profile; otherwise, false.</returns>
    public static bool CanEditProfile(this IEnumerable<Claim> claims, string profileUserId)
    {
        var claimList = claims.ToList();
        
        // Check if user is the owner
        var userIdClaim = claimList.FirstOrDefault(c => c.Type == "UserId");
        if (userIdClaim != null && userIdClaim.Value == profileUserId)
        {
            return true;
        }

        // Check if user has Admin or Moderator role
        if (claims.HasElevatedRole())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the UserId from the claims.
    /// </summary>
    public static string? GetUserId(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
    }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    public static bool HasRole(this IEnumerable<Claim> claims, string role)
    {
        return claims.Any(c => 
            (c.Type == ClaimTypes.Role || c.Type == "role") && 
            c.Value.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if the user has an elevated role (Admin or Moderator).
    /// </summary>
    public static bool HasElevatedRole(this IEnumerable<Claim> claims)
    {
        return claims.HasRole(ApplicationRoles.Admin.ToValueString())
               || claims.HasRole(ApplicationRoles.Moderator.ToValueString());
    }

    /// <summary>
    /// Checks if the user is an Admin.
    /// </summary>
    public static bool IsAdmin(this IEnumerable<Claim> claims)
    {
        return claims.HasRole(ApplicationRoles.Admin.ToValueString());
    }

    /// <summary>
    /// Checks if the user is a Moderator.
    /// </summary>
    public static bool IsModerator(this IEnumerable<Claim> claims)
    {
        return claims.HasRole(ApplicationRoles.Moderator.ToValueString());
    }
}
