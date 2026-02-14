using API.DTOs.Auth;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Auth;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly IAuthService _authService;

    public RegisterEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description =
                "Creates a new user account and returns the user details with JWT token";
        });
        Description(x => x.Produces<RegisterResponse>(201).Produces(400));
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var (user, token) = await _authService.RegisterAsync(
            req.Email,
            req.Password,
            req.FirstName,
            req.LastName
        );

        if (user == null || token == null)
        {
            AddError(r => r.Email, "Email already exists");
            ThrowIfAnyErrors();
        }

        var response = new RegisterResponse
        {
            UserId = user!.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token!
        };

        await HttpContext.Response.SendAsync(response, 201);
    }
}
