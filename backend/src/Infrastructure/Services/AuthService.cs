using Core.Entities;
using Core.Events;
using Core.Repositories;
using Core.Services;
using FastEndpoints.Security;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IPublishEndpoint publishEndpoint,
        JwtOptions jwtOptions
    )
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _publishEndpoint = publishEndpoint;
        _jwtOptions = jwtOptions;
    }

    public async Task<(ApplicationUser? User, string? Token)> LoginAsync(
        string email,
        string password
    )
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return (null, null);

        if (!user.IsActive)
            return (null, null);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
            return (null, null);

        var token = await GenerateJwtTokenAsync(user);
        return (user, token);
    }

    public async Task<(ApplicationUser? User, string? Token)> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName
    )
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(email))
            return (null, null);

        // Create new user (without FirstName/LastName - they are now in UserProfile)
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Create user with password
        var createdUser = await _userRepository.CreateAsync(user, password);

        // Generate JWT token
        var token = await GenerateJwtTokenAsync(createdUser);

        // Publish UserCreatedEvent (UserProfile will be created from this event)
        await _publishEndpoint.Publish(
            new UserCreatedEvent
            {
                UserId = createdUser.Id,
                Email = createdUser.Email ?? string.Empty,
                FirstName = firstName,
                LastName = lastName
            }
        );

        return (createdUser, token);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _jwtOptions.SigningKey;
            o.ExpireAt = DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays);
            o.User.Claims.Add(("UserId", user.Id));
            o.User.Claims.Add(("Email", user.Email ?? string.Empty));

            // FirstName and LastName will come from UserProfile in future
            // For now, we'll need to fetch from UserProfile or adjust claims

            foreach (var role in roles)
            {
                o.User.Roles.Add(role);
            }
        });

        return token;
    }
}
