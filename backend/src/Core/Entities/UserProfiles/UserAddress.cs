using Core.Entities.ValueObjects;

namespace Core.Entities.UserProfiles;

/// <summary>
/// Entity representing a user's address - 1:N relationship with UserProfile.
/// Contains Address value object and address type classification.
/// </summary>
public class UserAddress : BaseEntity
{
    /// <summary>
    /// Type of address (e.g., "Home", "Work", "Postal", "Billing")
    /// </summary>
    public string Type { get; set; } = "Home";

    /// <summary>
    /// The address details (value object)
    /// </summary>
    public Address Address { get; set; } = Address.Empty;

    // For EF Core navigation
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
}