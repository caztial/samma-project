using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for session real-time communication.
/// Clients only use this to join/leave groups. All event notifications are sent from backend event consumers.
/// </summary>
public class SessionHub : Hub
{
    private readonly ILogger<SessionHub> _logger;

    public SessionHub(ILogger<SessionHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join a session group to receive session events (for participants).
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize]
    public async Task JoinSessionGroup(string sessionCode)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"{sessionCode}-{Context.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value}"
            );
            _logger.LogInformation(
                "Client {ConnectionId} joined session group: {SessionCode}",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining session group: {SessionCode}", sessionCode);
            throw;
        }
    }

    /// <summary>
    /// Leave a session group.
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize]
    public async Task LeaveSessionGroup(string sessionCode)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionCode);
            _logger.LogInformation(
                "Client {ConnectionId} left session group: {SessionCode}",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving session group: {SessionCode}", sessionCode);
            throw;
        }
    }

    /// <summary>
    /// Join the admin group for a session to receive admin-only events (join/leave/answer notifications).
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize(Roles = "Admin,Moderator")]
    public async Task JoinAdminGroup(string sessionCode)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{sessionCode}-admin");
            _logger.LogInformation(
                "Admin client {ConnectionId} joined admin group: {SessionCode}-admin",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining admin group: {SessionCode}", sessionCode);
            throw;
        }
    }

    /// <summary>
    /// Leave the admin group for a session.
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize(Roles = "Admin,Moderator")]
    public async Task LeaveAdminGroup(string sessionCode)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{sessionCode}-admin");
            _logger.LogInformation(
                "Admin client {ConnectionId} left admin group: {SessionCode}-admin",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving admin group: {SessionCode}", sessionCode);
            throw;
        }
    }

    /// <summary>
    /// Join the presenter group for a session (for projector display).
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize(Roles = "Admin,Moderator,Presenter")]
    public async Task JoinPresenterGroup(string sessionCode)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{sessionCode}-presenter");
            _logger.LogInformation(
                "Presenter client {ConnectionId} joined presenter group: {SessionCode}-presenter",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining presenter group: {SessionCode}", sessionCode);
            throw;
        }
    }

    /// <summary>
    /// Leave the presenter group for a session.
    /// </summary>
    /// <param name="sessionCode">The unique session code</param>
    [Authorize(Roles = "Admin,Moderator,Presenter")]
    public async Task LeavePresenterGroup(string sessionCode)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{sessionCode}-presenter");
            _logger.LogInformation(
                "Presenter client {ConnectionId} left presenter group: {SessionCode}-presenter",
                Context.ConnectionId,
                sessionCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving presenter group: {SessionCode}", sessionCode);
            throw;
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation(
            "Client disconnected: {ConnectionId}, Exception: {Exception}",
            Context.ConnectionId,
            exception?.Message
        );
        await base.OnDisconnectedAsync(exception);
    }
}
