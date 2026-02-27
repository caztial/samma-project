using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to list sessions with pagination.
/// </summary>
public class ListSessionsEndpoint : Endpoint<SessionListRequest, SessionListResponse>
{
    private readonly ISessionService _sessionService;

    public ListSessionsEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "List sessions";
            s.Description = "Gets a paginated list of sessions with optional filtering.";
        });
    }

    public override async Task HandleAsync(SessionListRequest req, CancellationToken ct)
    {
        var (items, totalCount) = await _sessionService.GetAllAsync(
            req.PageNumber,
            req.PageSize,
            req.State,
            req.CreatedBy
        );

        var mapper = new SessionMapper();
        var response = new SessionListResponse
        {
            Items = items.Select(mapper.FromEntity).ToList(),
            TotalCount = totalCount,
            PageNumber = req.PageNumber,
            PageSize = req.PageSize
        };

        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}