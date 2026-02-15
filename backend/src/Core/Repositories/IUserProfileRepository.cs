using Core.Entities.UserProfiles;

namespace Core.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<UserProfile> UpdateAsync(UserProfile userProfile);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string userId);
}
