namespace Core.Enums;

/// <summary>
/// Application roles for authorization.
/// </summary>
public enum ApplicationRoles
{
    Admin = 1,
    Moderator = 2,
    Presenter = 3,
    Participant = 4
}

/// <summary>
/// Extension methods for ApplicationRoles.
/// </summary>
public static class ApplicationRolesExtensions
{
    /// <summary>
    /// Gets the string value of the role.
    /// </summary>
    public static string ToValueString(this ApplicationRoles role)
    {
        return role switch
        {
            ApplicationRoles.Admin => "Admin",
            ApplicationRoles.Moderator => "Moderator",
            ApplicationRoles.Presenter => "Presenter",
            ApplicationRoles.Participant => "Participant",
            _ => role.ToString()
        };
    }
}
