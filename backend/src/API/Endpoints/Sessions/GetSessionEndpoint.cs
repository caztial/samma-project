using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to get a session by ID.
/// </summary>
public class GetSessionEndpoint : Endpoint<SessionIdRequest, SessionResponse>
{
    private readonly ISessionService _sessionService;

    public GetSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}");
        Summary(s =>
        {
            s.Summary = "Get session by ID";
            s.Description = "Gets session details by ID.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var session = await _sessionService.GetByIdAsync(req.Id);

        if (session == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Session not found" },
                404,
                cancellation: ct
            );
            return;
        }

        var mapper = new SessionMapper();
        await HttpContext.Response.SendAsync(mapper.FromEntity(session), cancellation: ct);
    }
}
