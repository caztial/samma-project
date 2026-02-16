using System.Security.Claims;

namespace Core.Services;

/// <summary>
/// Interface for resource ownership authorization.
/// Implementations check if the current user is the owner of a resource.
/// </summary>
public interface IResourceOwnerAuthorization
{
    /// <summary>
    /// Checks if the current user is the owner of the resource.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="claims">The user's claims principal.</param>
    /// <returns>True if the user is the owner (or has permission); otherwise, false.</returns>
    Task<bool> IsOwnerAsync(Guid id, ClaimsPrincipal claims);
}
