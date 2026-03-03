using Microsoft.AspNetCore.Authorization;

namespace Core.Authorization;

/// <summary>
/// Authorization requirement that checks if the user has Admin or Moderator role.
/// Used for endpoints that should only be accessible by administrators and moderators.
/// </summary>
public class AdminModeratorRequirement : IAuthorizationRequirement { }
