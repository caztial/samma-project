using API.DTOs.Auth;
using Core.Entities;
using Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace API.Endpoints.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;
    private readonly IUserProfileService _userProfileService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginEndpoint(
        IAuthService authService,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager
    )
    {
        _authService = authService;
        _userProfileService = userProfileService;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "User login";
            s.Description = "Authenticates user and returns JWT token";
        });
        Description(x => x.Produces<LoginResponse>(200).Produces(401));
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var (user, token) = await _authService.LoginAsync(req.Email, req.Password);

        if (user == null || token == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Invalid email or password" }, 401);
            return;
        }

        // Fetch UserProfile to get FirstName and LastName (encrypted)
        var userProfile = await _userProfileService.GetByUserIdAsync(user.Id);

        // Fetch user roles
        var roles = await _userManager.GetRolesAsync(user);

        var response = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            ProfileId = userProfile?.Id.ToString() ?? string.Empty,
            FirstName = userProfile?.FirstName ?? string.Empty,
            LastName = userProfile?.LastName ?? string.Empty,
            Token = token,
            Roles = roles.ToList()
        };

        await HttpContext.Response.SendAsync(response, 200);
    }
}
