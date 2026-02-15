using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

/// <summary>
/// ApplicationUser - Identity user with minimal properties.
/// User profile data is now in UserProfile aggregate root.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
