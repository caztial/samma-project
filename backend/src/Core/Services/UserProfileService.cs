using Core.Entities.UserProfiles;
using Core.Events;
using Core.Repositories;

namespace Core.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfile> CreateFromEventAsync(UserCreatedEvent userEvent)
    {
        // Check if profile already exists
        if (await _userProfileRepository.ExistsAsync(userEvent.UserId))
        {
            var existing = await _userProfileRepository.GetByUserIdAsync(userEvent.UserId);
            if (existing != null)
            {
                return existing;
            }
        }

        // Create UserProfile from event
        var userProfile = UserProfile.CreateFromEvent(userEvent);

        // Save to database
        return await _userProfileRepository.CreateAsync(userProfile);
    }

    public async Task<UserProfile?> GetByUserIdAsync(string userId)
    {
        return await _userProfileRepository.GetByUserIdAsync(userId);
    }
}
