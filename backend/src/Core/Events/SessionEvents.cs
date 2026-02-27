namespace Core.Events;

/// <summary>
/// Event published when a new session is created.
/// </summary>
public record SessionCreatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}

/// <summary>
/// Event published when a session is activated.
/// </summary>
public record SessionActivatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
}

/// <summary>
/// Event published when a session is ended.
/// </summary>
public record SessionEndedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
}

/// <summary>
/// Event published when a participant joins a session.
/// </summary>
public record ParticipantJoinedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}

/// <summary>
/// Event published when a participant leaves a session.
/// </summary>
public record ParticipantLeftEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public DateTime LeftAt { get; init; }
}

/// <summary>
/// Event published when a question is assigned to a session.
/// </summary>
public record QuestionAssignedToSessionEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public Guid QuestionId { get; init; }
}

/// <summary>
/// Event published when a question is presented to participants.
/// </summary>
public record QuestionPresentedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public Guid QuestionId { get; init; }
    public Guid SessionQuestionId { get; init; }
    public bool ShowTitle { get; init; }
    public bool ShowOptionValues { get; init; }
    public int MaxAttempts { get; init; }
    public int? DurationSeconds { get; init; }
}

/// <summary>
/// Event published when a question attempt is activated.
/// </summary>
public record QuestionAttemptActivatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public Guid QuestionId { get; init; }
    public Guid SessionQuestionId { get; init; }
    public int AttemptNumber { get; init; }
    public double ScoreMultiplier { get; init; }
}

/// <summary>
/// Event published when a question is deactivated.
/// </summary>
public record QuestionDeactivatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public Guid QuestionId { get; init; }
    public Guid SessionQuestionId { get; init; }
}

/// <summary>
/// Event published when a participant submits an answer.
/// Used for batch processing to send notifications to admins.
/// </summary>
public record AnswerSubmittedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public Guid QuestionId { get; init; }
    public Guid SessionQuestionId { get; init; }
    public Guid AnswerId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public int AttemptNumber { get; init; }
    public bool IsCorrect { get; init; }
    public double Score { get; init; }
}

/// <summary>
/// Event published periodically to batch notify admins of new answers.
/// </summary>
public record AnswerBatchEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public Guid SessionQuestionId { get; init; }
    public int AnswerCount { get; init; }
    public IEnumerable<Guid> AnswerIds { get; init; } = [];
}

/// <summary>
/// Command to submit an MCQ answer asynchronously.
/// Used for high-frequency answer submission with queue-based processing.
/// </summary>
public record SubmitAnswerCommand
{
    /// <summary>
    /// Unique identifier for this command (used for idempotency).
    /// </summary>
    public Guid CommandId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// When the command was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// The session ID.
    /// </summary>
    public Guid SessionId { get; init; }

    /// <summary>
    /// The question ID.
    /// </summary>
    public Guid QuestionId { get; init; }

    /// <summary>
    /// The attempt number (1-based).
    /// </summary>
    public int AttemptNumber { get; init; }

    /// <summary>
    /// The selected MCQ option ID.
    /// </summary>
    public Guid SelectedOptionId { get; init; }

    /// <summary>
    /// The user submitting the answer.
    /// </summary>
    public string UserId { get; init; } = string.Empty;
}

/// <summary>
/// Event published when an answer submission is completed (success or failure).
/// Sent via SignalR to notify the submitting user.
/// </summary>
public record AnswerSubmissionResultEvent
{
    /// <summary>
    /// The original command ID for correlation.
    /// </summary>
    public Guid CommandId { get; init; }

    /// <summary>
    /// When the result was generated.
    /// </summary>
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// The session ID.
    /// </summary>
    public Guid SessionId { get; init; }

    /// <summary>
    /// The question ID.
    /// </summary>
    public Guid QuestionId { get; init; }

    /// <summary>
    /// Whether the submission was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Error message if unsuccessful.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// The answer ID if successful.
    /// </summary>
    public Guid? AnswerId { get; init; }

    /// <summary>
    /// The attempt number.
    /// </summary>
    public int AttemptNumber { get; init; }

    /// <summary>
    /// The selected option ID.
    /// </summary>
    public Guid SelectedOptionId { get; init; }

    /// <summary>
    /// Whether the answer was correct.
    /// </summary>
    public bool IsCorrect { get; init; }

    /// <summary>
    /// The score achieved.
    /// </summary>
    public double Score { get; init; }
}
