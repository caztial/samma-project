using Core.Entities;

namespace Core.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser> UpdateAsync(ApplicationUser user);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> EmailExistsAsync(string email);
}
