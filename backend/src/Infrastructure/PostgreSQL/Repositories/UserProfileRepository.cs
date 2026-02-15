using Core.Entities.UserProfiles;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;

    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByUserIdAsync(string userId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<UserProfile> CreateAsync(UserProfile userProfile)
    {
        userProfile.CreatedAt = DateTime.UtcNow;
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
    {
        userProfile.UpdatedAt = DateTime.UtcNow;
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null)
            return false;

        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        return await _context.UserProfiles.AnyAsync(up => up.UserId == userId);
    }
}
