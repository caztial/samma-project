using API.DTOs.Auth;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
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

        // FirstName and LastName are now in UserProfile (PII - encrypted)
        // For now, we'll need to fetch from UserProfile if needed
        var response = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = string.Empty, // TODO: Fetch from UserProfile
            LastName = string.Empty, // TODO: Fetch from UserProfile
            Token = token
        };

        await HttpContext.Response.SendAsync(response, 200);
    }
}
