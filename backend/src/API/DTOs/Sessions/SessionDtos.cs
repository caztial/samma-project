using Core.Enums;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs.Sessions;

/// <summary>
/// Request to create a new session.
/// </summary>
public record CreateSessionRequest
{
    /// <summary>
    /// The name of the session.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional location where the session takes place.
    /// </summary>
    public string? Location { get; set; }
}

/// <summary>
/// Request to update a session.
/// </summary>
public record UpdateSessionRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
}

/// <summary>
/// Request to join a session.
/// </summary>
public record JoinSessionRequest
{
    /// <summary>
    /// The unique session code.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request to assign a question to a session.
/// </summary>
public record AssignQuestionRequest
{
    public Guid QuestionId { get; set; }
    public int? Order { get; set; }
}

/// <summary>
/// Request to present a question.
/// </summary>
public record PresentQuestionRequest
{
    public Guid QuestionId { get; set; }
    public bool ShowTitle { get; set; } = true;
    public bool ShowOptionValues { get; set; } = true;
    public int MaxAttempts { get; set; } = 3;
    public int? CustomDurationSeconds { get; set; }
}

/// <summary>
/// Request to submit an MCQ answer (legacy - use SubmitMCQAnswerRequestBody with route params).
/// </summary>
public record SubmitMCQAnswerRequest
{
    public Guid QuestionId { get; set; }
    public int AttemptNumber { get; set; }
    public Guid SelectedOptionId { get; set; }
}

/// <summary>
/// Request body for submitting an MCQ answer.
/// Used with route params: POST /sessions/{id}/questions/{questionId}/attempts/{attemptNumber}/answers
/// </summary>
public record SubmitMCQAnswerRequestBody
{
    /// <summary>
    /// The selected MCQ option ID.
    /// </summary>
    public Guid SelectedOptionId { get; set; }
}

/// <summary>
/// Response for an accepted answer submission (async processing).
/// </summary>
public record SubmitAnswerAcceptedResponse
{
    /// <summary>
    /// The command ID for tracking/correlation.
    /// </summary>
    public Guid CommandId { get; set; }

    /// <summary>
    /// The status of the submission.
    /// </summary>
    public string Status { get; set; } = "Accepted";

    /// <summary>
    /// Human-readable message.
    /// </summary>
    public string Message { get; set; } =
        "Answer submission is being processed. You will be notified via SignalR.";

    /// <summary>
    /// The session ID.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// The question ID.
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// The attempt number.
    /// </summary>
    public int AttemptNumber { get; set; }
}

/// <summary>
/// Response for a session.
/// </summary>
public record SessionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? State { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response for a session participant.
/// </summary>
public record SessionParticipantResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
}

/// <summary>
/// Response for a session question.
/// </summary>
public record SessionQuestionResponse
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionNumber { get; set; } = string.Empty;
    public string? QuestionText { get; set; }
    public int Order { get; set; }
    public bool IsPresented { get; set; }
    public DateTime? PresentedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
}

/// <summary>
/// Response for a presented question with active attempt.
/// </summary>
public record PresentedQuestionResponse
{
    public Guid SessionQuestionId { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionNumber { get; set; } = string.Empty;
    public string? QuestionText { get; set; }
    public string? QuestionDescription { get; set; }
    public bool ShowTitle { get; set; }
    public bool ShowOptionValues { get; set; }
    public int MaxAttempts { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime? PresentedAt { get; set; }
    public ActiveAttemptResponse? ActiveAttempt { get; set; }
    public List<AvailableAttemptResponse> AvailableAttempts { get; set; } = [];
}

public record PresentedMcqQuestionResponse
{
    public Guid SessionQuestionId { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionNumber { get; set; } = string.Empty;
    public string? QuestionText { get; set; }
    public string? QuestionDescription { get; set; }
    public List<PresentedMcqAnswerResponse> Options { get; set; } = [];
    public bool ShowTitle { get; set; }
    public bool ShowOptionValues { get; set; }
    public int MaxAttempts { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime? PresentedAt { get; set; }
    public ActiveAttemptResponse? ActiveAttempt { get; set; }
    public List<AvailableAttemptResponse> AvailableAttempts { get; set; } = [];
}

public record PresentedMcqAnswerResponse
{
    public Guid OptionId { get; set; }
    public string OptionNumber { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public int Order { get; set; }
}

/// <summary>
/// Response for an active attempt.
/// </summary>
public record ActiveAttemptResponse
{
    public int AttemptNumber { get; set; }
    public double ScoreMultiplier { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

/// <summary>
/// Response for an available attempt.
/// </summary>
public record AvailableAttemptResponse
{
    public int AttemptNumber { get; set; }
    public bool IsActive { get; set; }
    public double ScoreMultiplier { get; set; }
}

/// <summary>
/// Response for a submitted answer.
/// </summary>
public record SubmitAnswerResponse
{
    public Guid AnswerId { get; set; }
    public Guid QuestionId { get; set; }
    public int AttemptNumber { get; set; }
    public Guid SelectedOptionId { get; set; }
    public bool IsCorrect { get; set; }
    public int BasePoints { get; set; }
    public double FinalScore { get; set; }
}

/// <summary>
/// Response for session scores.
/// </summary>
public record SessionScoresResponse
{
    public Guid SessionId { get; set; }
    public List<ParticipantScoreResponse> Scores { get; set; } = [];
}

/// <summary>
/// Response for a participant's score.
/// </summary>
public record ParticipantScoreResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public double TotalScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalAttempts { get; set; }
}

/// <summary>
/// Response for question scores.
/// </summary>
public record QuestionScoresResponse
{
    public Guid SessionQuestionId { get; set; }
    public Guid QuestionId { get; set; }
    public List<QuestionParticipantScoreResponse> Scores { get; set; } = [];
}

/// <summary>
/// Response for a participant's score on a specific question.
/// </summary>
public record QuestionParticipantScoreResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public double Score { get; set; }
    public bool IsCorrect { get; set; }
    public int AttemptNumber { get; set; }
}

/// <summary>
/// Response for a list of sessions.
/// </summary>
public record SessionListResponse
{
    public List<SessionResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Request for session list with query parameters.
/// </summary>
public record SessionListRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SessionState? State { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Request with session ID route parameter.
/// </summary>
public record SessionIdRequest
{
    [FromRoute]
    public Guid Id { get; set; }
}

/// <summary>
/// Request for question attempt activation.
/// </summary>
public record ActivateAttemptRequest
{
    public Guid Id { get; set; } // Session ID
    public Guid QuestionId { get; set; }
    public int AttemptNumber { get; set; }
}
