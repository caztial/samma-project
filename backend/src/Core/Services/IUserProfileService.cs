using Core.Entities.UserProfiles;
using Core.Events;

namespace Core.Services;

public interface IUserProfileService
{
    /// <summary>
    /// Creates a new UserProfile from a UserCreatedEvent and saves it to the database.
    /// </summary>
    Task<UserProfile> CreateFromEventAsync(UserCreatedEvent userEvent);

    /// <summary>
    /// Gets UserProfile by user ID.
    /// </summary>
    Task<UserProfile?> GetByUserIdAsync(string userId);
}
