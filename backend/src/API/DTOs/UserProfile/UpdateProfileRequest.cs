using FastEndpoints;

namespace API.DTOs.UserProfile;

/// <summary>
/// Request DTO for updating user profile.
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// User ID from JWT claims. Used when profile ID is not provided in route.
    /// </summary>
    [FromClaim("UserId")]
    public string? UserId { get; set; }

    /// <summary>
    /// First name of the user. Required, minimum 3 characters.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of the user. Optional.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// URL to the profile image.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gender as string (Male, Female, Other, PreferNotToSay). Required.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Date of birth as string (YYYY-MM-DD format). Required.
    /// </summary>
    public string? DateOfBirth { get; set; }

    /// <summary>
    /// Contact number. Optional.
    /// </summary>
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Email address. Optional.
    /// </summary>
    public string? Email { get; set; }
}
