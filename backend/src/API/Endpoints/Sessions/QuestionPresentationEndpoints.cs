using API.DTOs.Sessions;
using API.Mappers;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to present a question to participants.
/// </summary>
public class PresentQuestionEndpoint : Endpoint<PresentQuestionRequest, PresentedQuestionResponse>
{
    private readonly ISessionService _sessionService;

    public PresentQuestionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/questions/{QuestionId}/present");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Present a question";
            s.Description = "Presents a question to participants with specified options.";
        });
    }

    public override async Task HandleAsync(PresentQuestionRequest req, CancellationToken ct)
    {
        var sessionId = Route<Guid>("Id");
        var questionId = Route<Guid>("QuestionId");

        try
        {
            var sessionQuestion = await _sessionService.PresentQuestionAsync(
                sessionId,
                questionId,
                req.ShowTitle,
                req.ShowOptionValues,
                req.MaxAttempts,
                req.CustomDurationSeconds
            );

            var mapper = new PresentedMcqQuestionMapper();
            await HttpContext.Response.SendAsync(
                mapper.FromEntity(sessionQuestion),
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
/// Endpoint to deactivate a presented question.
/// </summary>
public class DeactivateQuestionEndpoint : Endpoint<SessionIdRequest>
{
    private readonly ISessionService _sessionService;

    public DeactivateQuestionEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/questions/{QuestionId}/deactivate");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Deactivate a question";
            s.Description = "Stops accepting answers for a presented question.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var questionId = Route<Guid>("QuestionId");

        var sessionQuestion = await _sessionService.DeactivateQuestionAsync(req.Id, questionId);

        if (sessionQuestion == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question not found or not active" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Question deactivated successfully" },
            cancellation: ct
        );
    }
}

/// <summary>
/// Endpoint to activate a specific attempt.
/// </summary>
public class ActivateAttemptEndpoint : Endpoint<ActivateAttemptRequest>
{
    private readonly ISessionService _sessionService;

    public ActivateAttemptEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/questions/{QuestionId}/attempts/{AttemptNumber}/activate");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Activate an attempt";
            s.Description = "Activates a specific attempt for a question.";
        });
    }

    public override async Task HandleAsync(ActivateAttemptRequest req, CancellationToken ct)
    {
        try
        {
            var attempt = await _sessionService.ActivateAttemptAsync(
                req.Id,
                req.QuestionId,
                req.AttemptNumber
            );
            await HttpContext.Response.SendAsync(
                new
                {
                    attempt.SessionQuestionId,
                    attempt.AttemptNumber,
                    ScoreMultiplier = attempt.GetScoreMultiplier(),
                    attempt.ActivatedAt
                },
                cancellation: ct
            );
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}
