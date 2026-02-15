using Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Security;

/// <summary>
/// Authorization handler for profile owner verification.
/// Checks if the user is the owner of the profile OR has Admin/Moderator role.
/// </summary>
public class ProfileOwnerAuthorizationHandler : IAuthorizationHandler
{
    private readonly IUserProfileService _userProfileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileOwnerAuthorizationHandler(
        IUserProfileService userProfileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userProfileService = userProfileService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        // Try to get profile ID from route - check various route parameters
        var profileId = GetProfileIdFromRoute(httpContext);
        if (!profileId.HasValue)
        {
            // If no profile ID in route, fail authorization
            context.Fail();
            return;
        }

        // Get the profile
        var profile = await _userProfileService.GetByIdAsync(profileId.Value);
        if (profile == null)
        {
            // Profile not found - fail authorization
            context.Fail();
            return;
        }

        // Get current user's ID from claims
        var userIdClaim = context.User.FindFirst("UserId")?.Value;
        
        // Check if user is owner OR has Admin/Moderator role
        var isOwner = userIdClaim == profile.UserId;
        var isAdminOrModerator = context.User.IsInRole("Admin") || context.User.IsInRole("Moderator");

        if (isOwner || isAdminOrModerator)
        {
            // Succeed for all requirements of type ProfileOwnerRequirement
            foreach (var requirement in context.Requirements.OfType<ProfileOwnerRequirement>())
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            context.Fail();
        }
    }

    private Guid? GetProfileIdFromRoute(HttpContext httpContext)
    {
        // Check common route parameter names for profile ID
        var routeKeys = new[] { "id", "Id", "profileId", "ProfileId" };

        foreach (var key in routeKeys)
        {
            if (httpContext.Request.RouteValues.TryGetValue(key, out var value))
            {
                if (value is Guid guidValue)
                    return guidValue;
                
                if (Guid.TryParse(value?.ToString(), out var parsedGuid))
                    return parsedGuid;
            }
        }

        return null;
    }
}

/// <requirement>
/// Dummy requirement class for the policy
/// </requirement>
public class ProfileOwnerRequirement : IAuthorizationRequirement
{
}
