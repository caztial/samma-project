using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to activate a session.
/// </summary>
public class ActivateSessionEndpoint : EndpointWithoutRequest<SessionResponse>
{
    private readonly ISessionService _sessionService;

    public ActivateSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/activate");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Activate a session";
            s.Description = "Activates a session allowing participants to join.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid sessionId = Route<Guid>("Id");
        var session = await _sessionService.ActivateAsync(sessionId);

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

/// <summary>
/// Endpoint to deactivate (pause) a session.
/// </summary>
public class DeactivateSessionEndpoint : EndpointWithoutRequest<SessionResponse>
{
    private readonly ISessionService _sessionService;

    public DeactivateSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/deactivate");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Deactivate (pause) a session";
            s.Description = "Pauses an active session.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid sessionId = Route<Guid>("Id");
        var session = await _sessionService.DeactivateAsync(sessionId);

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

/// <summary>
/// Endpoint to reactivate a paused session.
/// </summary>
public class ReactivateSessionEndpoint : EndpointWithoutRequest<SessionResponse>
{
    private readonly ISessionService _sessionService;

    public ReactivateSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/reactivate");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Reactivate a paused session";
            s.Description = "Resumes a paused session to active state.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid sessionId = Route<Guid>("Id");
        var session = await _sessionService.ReactivateAsync(sessionId);

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

/// <summary>
/// Endpoint to end a session.
/// </summary>
public class EndSessionEndpoint : EndpointWithoutRequest<SessionResponse>
{
    private readonly ISessionService _sessionService;

    public EndSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/end");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "End a session";
            s.Description = "Ends a session permanently.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid sessionId = Route<Guid>("Id");
        var session = await _sessionService.EndAsync(sessionId);

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
