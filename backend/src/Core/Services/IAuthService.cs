using Core.Entities;

namespace Core.Services;

public interface IAuthService
{
    Task<(ApplicationUser? User, string? Token)> LoginAsync(string email, string password);
    Task<(ApplicationUser? User, string? Token)> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName
    );
}
