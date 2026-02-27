using API.Hubs;
using Core.Events;
using Core.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Consumers;

/// <summary>
/// Consumer for ParticipantJoinedEvent - sends SSE notification to Admin/Moderators.
/// </summary>
public class ParticipantJoinedEventConsumer : IConsumer<ParticipantJoinedEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<ParticipantJoinedEventConsumer> _logger;

    public ParticipantJoinedEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<ParticipantJoinedEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ParticipantJoinedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing ParticipantJoinedEvent: SessionId={SessionId}, UserId={UserId}",
            evt.SessionId,
            evt.UserId
        );

        // Send notification to admin group for this session
        var notification = new
        {
            EventType = "ParticipantJoined",
            evt.SessionId,
            Timestamp = evt.OccurredAt
        };

        await _hubContext
            .Clients.Group($"{evt.Code}-admin")
            .SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: ParticipantJoined to admin group");
    }
}

/// <summary>
/// Consumer for ParticipantLeftEvent - sends SSE notification to Admin/Moderators.
/// </summary>
public class ParticipantLeftEventConsumer : IConsumer<ParticipantLeftEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<ParticipantLeftEventConsumer> _logger;

    public ParticipantLeftEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<ParticipantLeftEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ParticipantLeftEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing ParticipantLeftEvent: SessionId={SessionId}, UserId={UserId}",
            evt.SessionId,
            evt.UserId
        );

        // Send notification to admin group for this session
        var notification = new
        {
            EventType = "ParticipantLeft",
            evt.SessionId,
            Timestamp = evt.OccurredAt
        };

        await _hubContext
            .Clients.Group($"{evt.Code}-admin")
            .SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: ParticipantLeft to admin group");
    }
}

/// <summary>
/// Consumer for QuestionPresentedEvent - sends SSE notification to Participants.
/// </summary>
public class QuestionPresentedEventConsumer : IConsumer<QuestionPresentedEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<QuestionPresentedEventConsumer> _logger;

    public QuestionPresentedEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<QuestionPresentedEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QuestionPresentedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing QuestionPresentedEvent: SessionId={SessionId}, QuestionId={QuestionId}",
            evt.SessionId,
            evt.QuestionId
        );

        // Send notification to all participants in the session
        var notification = new
        {
            EventType = "QuestionPresented",
            evt.SessionId,
            evt.SessionQuestionId,
            Timestamp = evt.OccurredAt
        };

        await _hubContext.Clients.Group(evt.Code).SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: QuestionPresented to participants");
    }
}

/// <summary>
/// Consumer for QuestionAttemptActivatedEvent - sends SSE notification to Participants.
/// </summary>
public class QuestionAttemptActivatedEventConsumer : IConsumer<QuestionAttemptActivatedEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<QuestionAttemptActivatedEventConsumer> _logger;

    public QuestionAttemptActivatedEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<QuestionAttemptActivatedEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QuestionAttemptActivatedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing QuestionAttemptActivatedEvent: SessionId={SessionId}, Attempt={AttemptNumber}",
            evt.SessionId,
            evt.AttemptNumber
        );

        // Send notification to all participants in the session
        var notification = new
        {
            EventType = "AttemptActivated",
            evt.SessionId,
            evt.SessionQuestionId,
            evt.AttemptNumber,
            evt.ScoreMultiplier,
            Timestamp = evt.OccurredAt
        };

        await _hubContext.Clients.Group(evt.Code).SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: AttemptActivated to participants");
    }
}

/// <summary>
/// Consumer for QuestionDeactivatedEvent - sends SSE notification to Participants.
/// </summary>
public class QuestionDeactivatedEventConsumer : IConsumer<QuestionDeactivatedEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<QuestionDeactivatedEventConsumer> _logger;

    public QuestionDeactivatedEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<QuestionDeactivatedEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QuestionDeactivatedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing QuestionDeactivatedEvent: SessionId={SessionId}, QuestionId={QuestionId}",
            evt.SessionId,
            evt.QuestionId
        );

        // Send notification to all participants in the session
        var notification = new
        {
            EventType = "QuestionDeactivated",
            evt.SessionId,
            evt.SessionQuestionId,
            Timestamp = evt.OccurredAt
        };

        await _hubContext.Clients.Group(evt.Code).SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: QuestionDeactivated to participants");
    }
}

/// <summary>
/// Consumer for SessionEndedEvent - sends SSE notification to all users.
/// </summary>
public class SessionEndedEventConsumer : IConsumer<SessionEndedEvent>
{
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<SessionEndedEventConsumer> _logger;

    public SessionEndedEventConsumer(
        IHubContext<SessionHub> hubContext,
        ILogger<SessionEndedEventConsumer> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SessionEndedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Processing SessionEndedEvent: SessionId={SessionId}",
            evt.SessionId
        );

        // Send notification to all users in the session (participants and admins)
        var notification = new
        {
            EventType = "SessionEnded",
            evt.SessionId,
            Timestamp = evt.OccurredAt
        };

        // Send to both participants and admin groups
        await _hubContext.Clients.Group(evt.Code).SendAsync("SessionEvent", notification);
        await _hubContext
            .Clients.Group($"{evt.Code}-admin")
            .SendAsync("SessionEvent", notification);

        _logger.LogDebug("SSE notification sent: SessionEnded to all users");
    }
}

/// <summary>
/// Consumer for AnswerSubmittedEvent - stores answers for batching.
/// This consumer works with the AnswerBatchingService to batch notifications.
/// </summary>
public class AnswerSubmittedEventConsumer : IConsumer<AnswerSubmittedEvent>
{
    private readonly ILogger<AnswerSubmittedEventConsumer> _logger;

    public AnswerSubmittedEventConsumer(ILogger<AnswerSubmittedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<AnswerSubmittedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "Answer submitted: SessionId={SessionId}, QuestionId={QuestionId}, UserId={UserId}, Attempt={Attempt}, Score={Score}",
            evt.SessionId,
            evt.QuestionId,
            evt.UserId,
            evt.AttemptNumber,
            evt.Score
        );

        // The actual batched notification is handled by AnswerBatchingService
        // This consumer just logs the event for audit purposes
        return Task.CompletedTask;
    }
}

/// <summary>
/// Consumer for SubmitAnswerCommand - processes answer submissions asynchronously.
/// Handles idempotency checks and sends results via SignalR.
/// </summary>
public class SubmitAnswerCommandConsumer : IConsumer<SubmitAnswerCommand>
{
    private readonly ISessionService _sessionService;
    private readonly IHubContext<SessionHub> _hubContext;
    private readonly ILogger<SubmitAnswerCommandConsumer> _logger;

    public SubmitAnswerCommandConsumer(
        ISessionService sessionService,
        IHubContext<SessionHub> hubContext,
        ILogger<SubmitAnswerCommandConsumer> logger
    )
    {
        _sessionService = sessionService;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitAnswerCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation(
            "Processing SubmitAnswerCommand: CommandId={CommandId}, SessionId={SessionId}, UserId={UserId}",
            command.CommandId,
            command.SessionId,
            command.UserId
        );

        try
        {
            // Process the answer submission
            var answer = await _sessionService.SubmitMCQAnswerAsync(
                command.SessionId,
                command.QuestionId,
                command.UserId,
                command.AttemptNumber,
                command.SelectedOptionId
            );

            _logger.LogInformation(
                "Answer processed successfully: CommandId={CommandId}, AnswerId={AnswerId}, IsCorrect={IsCorrect}, Score={Score}",
                command.CommandId,
                answer.Id,
                answer.IsCorrect,
                answer.FinalScore
            );

            // Send success result via SignalR to the user
            var resultEvent = new AnswerSubmissionResultEvent
            {
                CommandId = command.CommandId,
                SessionId = command.SessionId,
                QuestionId = command.QuestionId,
                Success = true,
                AnswerId = answer.Id,
                AttemptNumber = command.AttemptNumber,
                SelectedOptionId = command.SelectedOptionId,
                IsCorrect = answer.IsCorrect,
                Score = answer.FinalScore
            };

            await _hubContext.Clients.User(command.UserId).SendAsync("AnswerResult", resultEvent);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Answer submission failed: CommandId={CommandId}, Error={Error}",
                command.CommandId,
                ex.Message
            );

            // Send error result via SignalR to the user
            var resultEvent = new AnswerSubmissionResultEvent
            {
                CommandId = command.CommandId,
                SessionId = command.SessionId,
                QuestionId = command.QuestionId,
                Success = false,
                ErrorMessage = ex.Message,
                AttemptNumber = command.AttemptNumber,
                SelectedOptionId = command.SelectedOptionId
            };

            await _hubContext.Clients.User(command.UserId).SendAsync("AnswerResult", resultEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error processing answer: CommandId={CommandId}",
                command.CommandId
            );

            // Send error result via SignalR to the user
            var resultEvent = new AnswerSubmissionResultEvent
            {
                CommandId = command.CommandId,
                SessionId = command.SessionId,
                QuestionId = command.QuestionId,
                Success = false,
                ErrorMessage = "An unexpected error occurred while processing your answer.",
                AttemptNumber = command.AttemptNumber,
                SelectedOptionId = command.SelectedOptionId
            };

            await _hubContext.Clients.User(command.UserId).SendAsync("AnswerResult", resultEvent);
        }
    }
}
