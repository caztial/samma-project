using API.DTOs.Sessions;
using API.Mappers;
using Core.Events;
using Core.Services;
using FastEndpoints;
using MassTransit;

namespace API.Endpoints.Sessions;

/// <summary>
/// Endpoint to submit an MCQ answer asynchronously.
/// Uses MassTransit queue for high-frequency answer processing.
/// Route: POST /sessions/{Id}/questions/{QuestionId}/attempts/{AttemptNumber}/answers
/// </summary>
public class SubmitMCQAnswerEndpoint
    : Endpoint<SubmitMCQAnswerRequestBody, SubmitAnswerAcceptedResponse>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitMCQAnswerEndpoint(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public override void Configure()
    {
        Post("/sessions/{Id}/questions/{QuestionId}/attempts/{AttemptNumber}/answers");
        Summary(s =>
        {
            s.Summary = "Submit an MCQ answer";
            s.Description =
                "Submit an answer for a presented question attempt. Processing is async - results sent via SignalR.";
            s.Response<SubmitAnswerAcceptedResponse>(202, "Answer accepted for processing");
            s.Response<object>(401, "Unauthorized");
        });
    }

    public override async Task HandleAsync(SubmitMCQAnswerRequestBody req, CancellationToken ct)
    {
        var sessionId = Route<Guid>("Id");
        var questionId = Route<Guid>("QuestionId");
        var attemptNumber = Route<int>("AttemptNumber");
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

        // Create the command with a unique ID for idempotency
        // Capture request received time for timeout validation
        var command = new SubmitAnswerCommand
        {
            SessionId = sessionId,
            QuestionId = questionId,
            AttemptNumber = attemptNumber,
            SelectedOptionId = req.SelectedOptionId,
            UserId = userId
        };

        // Publish to MassTransit queue for async processing
        await _publishEndpoint.Publish(command, ct);

        // Return accepted response immediately
        var response = new SubmitAnswerAcceptedResponse
        {
            CommandId = command.CommandId,
            SessionId = sessionId,
            QuestionId = questionId,
            AttemptNumber = attemptNumber
        };

        await HttpContext.Response.SendAsync(response, 202, cancellation: ct);
    }
}

/// <summary>
/// Endpoint to get session scores.
/// </summary>
public class GetSessionScoresEndpoint : Endpoint<SessionIdRequest, SessionScoresResponse>
{
    private readonly ISessionService _sessionService;

    public GetSessionScoresEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}/scores");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Get session scores";
            s.Description = "Gets total scores for all participants in a session.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var scores = await _sessionService.GetSessionScoresAsync(req.Id);

        var response = new SessionScoresResponse
        {
            SessionId = req.Id,
            Scores = scores
                .Select(kvp => new ParticipantScoreResponse
                {
                    UserId = kvp.Key,
                    UserName = kvp.Value.UserName,
                    TotalScore = kvp.Value.TotalScore,
                    CorrectAnswers = kvp.Value.CorrectAnswers,
                    TotalAttempts = kvp.Value.TotalAttempts
                })
                .ToList()
        };

        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}

/// <summary>
/// Endpoint to get question scores.
/// </summary>
public class GetQuestionScoresEndpoint : Endpoint<SessionIdRequest, QuestionScoresResponse>
{
    private readonly ISessionService _sessionService;

    public GetQuestionScoresEndpoint(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override void Configure()
    {
        Get("/sessions/{Id}/questions/{QuestionId}/scores");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Get question scores";
            s.Description = "Gets scores for all participants for a specific question.";
        });
    }

    public override async Task HandleAsync(SessionIdRequest req, CancellationToken ct)
    {
        var questionId = Route<Guid>("QuestionId");

        var scores = await _sessionService.GetQuestionScoresAsync(req.Id, questionId);

        var response = new QuestionScoresResponse
        {
            QuestionId = questionId,
            Scores = scores
                .Select(kvp => new QuestionParticipantScoreResponse
                {
                    UserId = kvp.Key,
                    UserName = kvp.Value.UserName,
                    Score = kvp.Value.Score,
                    IsCorrect = kvp.Value.IsCorrect,
                    AttemptNumber = kvp.Value.AttemptNumber
                })
                .ToList()
        };

        await HttpContext.Response.SendAsync(response, cancellation: ct);
    }
}
