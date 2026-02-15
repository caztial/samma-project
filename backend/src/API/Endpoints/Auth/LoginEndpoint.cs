using API.DTOs.Auth;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;
    private readonly IUserProfileService _userProfileService;

    public LoginEndpoint(IAuthService authService, IUserProfileService userProfileService)
    {
        _authService = authService;
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
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

        var response = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = userProfile?.FirstName ?? string.Empty,
            LastName = userProfile?.LastName ?? string.Empty,
            Token = token
        };

        await HttpContext.Response.SendAsync(response, 200);
    }
}
