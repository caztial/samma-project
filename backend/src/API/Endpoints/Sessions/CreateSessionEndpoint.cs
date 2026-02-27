using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to create a new session.
/// </summary>
public class CreateSessionEndpoint : Endpoint<CreateSessionRequest, SessionResponse, SessionMapper>
{
    private readonly ISessionService _sessionService;

    public CreateSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Create a new session";
            s.Description =
                "Creates a new session with a unique code. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(CreateSessionRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await HttpContext.Response.SendAsync(
                new { error = "UserId not exist" },
                400,
                cancellation: ct
            );
            return;
        }

        try
        {
            var session = await _sessionService.CreateAsync(req.Name, req.Location, userId);
            Response = Map.FromEntity(session);
            await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}
