using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to join a session.
/// </summary>
public class JoinSessionEndpoint : Endpoint<JoinSessionRequest, SessionParticipantResponse>
{
    private readonly ISessionService _sessionService;

    public JoinSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/join");
        Summary(s =>
        {
            s.Summary = "Join a session";
            s.Description = "Join an active session using the session code.";
        });
    }

    public override async Task HandleAsync(JoinSessionRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await HttpContext.Response.SendAsync(
                new { error = "Unauthorized" },
                401,
                cancellation: ct
            );
            return;
        }

        try
        {
            var participant = await _sessionService.JoinSessionAsync(req.Code, userId);
            var mapper = new SessionParticipantMapper();
            await HttpContext.Response.SendAsync(
                mapper.FromEntity(participant),
                201,
                cancellation: ct
            );
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}

/// <summary>
/// Endpoint to leave a session.
/// </summary>
public class LeaveSessionEndpoint : Endpoint<SessionIdRequest>
{
    private readonly ISessionService _sessionService;

    public LeaveSessionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/leave");
        Summary(s =>
        {
            s.Summary = "Leave a session";
            s.Description = "Leave a session you have joined.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await HttpContext.Response.SendAsync(
                new { error = "Unauthorized" },
                401,
                cancellation: ct
            );
            return;
        }

        var success = await _sessionService.LeaveSessionAsync(req.Id, userId);

        if (!success)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to leave session" },
                400,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Left session successfully" },
            cancellation: ct
        );
    }
}

/// <summary>
/// Endpoint to get session participants.
/// </summary>
public class GetParticipantsEndpoint : Endpoint<SessionIdRequest, List<SessionParticipantResponse>>
{
    private readonly ISessionService _sessionService;

    public GetParticipantsEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}/participants");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Get session participants";
            s.Description = "Gets all participants in a session.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var participants = await _sessionService.GetParticipantsAsync(req.Id);
        var mapper = new SessionParticipantMapper();
        var response = participants.Select(mapper.FromEntity).ToList();
        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}
