using System.Security.Claims;
using Core.Authorization;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace API.Security;

/// <summary>
/// Authorization handler for AdminModeratorRequirement.
/// Only allows users with Admin or Moderator role.
/// </summary>
public class AdminModeratorAuthorizationHandler : AuthorizationHandler<AdminModeratorRequirement>
{
    private readonly ILogger<AdminModeratorAuthorizationHandler> _logger;

    public AdminModeratorAuthorizationHandler(ILogger<AdminModeratorAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminModeratorRequirement requirement
    )
    {
        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning(
                "Unauthenticated user attempted to access resource requiring Admin/Moderator authorization"
            );
            context.Fail();
            return Task.CompletedTask;
        }

        // Check if user has Admin or Moderator role
        if (HasElevatedRole(context.User))
        {
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning(
                "User without Admin/Moderator role attempted to access restricted resource"
            );
            context.Fail();
        }

        return Task.CompletedTask;
    }

    private static bool HasElevatedRole(ClaimsPrincipal claims)
    {
        return claims.IsInRole(ApplicationRoles.Admin.ToValueString())
            || claims.IsInRole(ApplicationRoles.Moderator.ToValueString());
    }
}
