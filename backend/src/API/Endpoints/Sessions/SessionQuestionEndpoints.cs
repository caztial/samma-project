using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to assign a question to a session.
/// </summary>
public class AssignQuestionEndpoint : Endpoint<AssignQuestionRequest, SessionQuestionResponse>
{
    private readonly ISessionService _sessionService;

    public AssignQuestionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/questions");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Assign a question to a session";
            s.Description = "Adds a question to the session's question list.";
        });
    }

    public override async Task HandleAsync(AssignQuestionRequest req, CancellationToken ct)
    {
        var sessionId = Route<Guid>("Id");

        try
        {
            var sessionQuestion = await _sessionService.AssignQuestionAsync(
                sessionId,
                req.QuestionId,
                req.Order
            );
            var mapper = new SessionQuestionMapper();
            await HttpContext.Response.SendAsync(
                mapper.FromEntity(sessionQuestion),
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
/// Endpoint to remove a question from a session.
/// </summary>
public class RemoveQuestionEndpoint : Endpoint<SessionIdRequest>
{
    private readonly ISessionService _sessionService;

    public RemoveQuestionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Delete("/sessions/{Id}/questions/{QuestionId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Remove a question from a session";
            s.Description = "Removes a question from the session's question list.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var questionId = Route<Guid>("QuestionId");

        try
        {
            var success = await _sessionService.RemoveQuestionAsync(req.Id, questionId);

            if (!success)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "Question not found in session" },
                    404,
                    cancellation: ct
                );
                return;
            }

            await HttpContext.Response.SendAsync(
                new { message = "Question removed successfully" },
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
/// Endpoint to get all questions assigned to a session.
/// </summary>
public class GetSessionQuestionsEndpoint : Endpoint<SessionIdRequest, List<SessionQuestionResponse>>
{
    private readonly ISessionService _sessionService;

    public GetSessionQuestionsEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}/questions");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get session questions";
            s.Description = "Gets all questions assigned to a session.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var questions = await _sessionService.GetAssignedQuestionsAsync(req.Id);
        var mapper = new SessionQuestionMapper();
        var response = questions.Select(mapper.FromEntity).ToList();
        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}

/// <summary>
/// Endpoint to get currently presented questions with active attempts.
/// </summary>
public class GetPresentedQuestionsEndpoint
    : Endpoint<SessionIdRequest, List<PresentedQuestionResponse>>
{
    private readonly ISessionService _sessionService;

    public GetPresentedQuestionsEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}/presented");
        Summary(s =>
        {
            s.Summary = "Get presented questions";
            s.Description =
                "Gets currently presented questions with active attempts for the session.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var questions = await _sessionService.GetPresentedQuestionsAsync(req.Id);
        var mapper = new PresentedMcqQuestionMapper();
        var response = questions.Select(mapper.FromEntity).ToList();
        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}
