using Core.Entities.Questions;

namespace Core.Entities.Sessions;

/// <summary>
/// Represents a question assigned to a session with presentation options.
/// </summary>
public class SessionQuestion : BaseEntity
{
    /// <summary>
    /// The session this question belongs to.
    /// </summary>
    public Guid SessionId { get; private set; }

    /// <summary>
    /// The question that is assigned.
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// The order of this question in the session.
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Whether this question has been presented to participants.
    /// </summary>
    public bool IsPresented { get; private set; }

    /// <summary>
    /// When the question was presented.
    /// </summary>
    public DateTime? PresentedAt { get; private set; }

    /// <summary>
    /// When the question was deactivated (no longer accepting answers).
    /// </summary>
    public DateTime? DeactivatedAt { get; private set; }

    /// <summary>
    /// Whether to show the question title to participants.
    /// If false, only the question number will be displayed.
    /// </summary>
    public bool ShowTitle { get; private set; } = true;

    /// <summary>
    /// Whether to show MCQ option values to participants.
    /// If false, only option numbers will be displayed.
    /// </summary>
    public bool ShowOptionValues { get; private set; } = true;

    /// <summary>
    /// Maximum attempts allowed for this question.
    /// Default is 3.
    /// </summary>
    public int MaxAttempts { get; private set; } = 3;

    /// <summary>
    /// Custom duration for this question (overrides question's default).
    /// </summary>
    public int? CustomDurationSeconds { get; private set; }

    // Navigation properties
    public Session Session { get; private set; } = null!;
    public Question Question { get; set; } = null!;

    /// <summary>
    /// Attempts for this question.
    /// </summary>
    public virtual ICollection<QuestionAttempt> Attempts { get; private set; } = [];

    // EF Core parameterless constructor
    private SessionQuestion() { }

    /// <summary>
    /// Creates a new session question assignment.
    /// </summary>
    public SessionQuestion(Guid sessionId, Guid questionId, int order)
    {
        SessionId = sessionId;
        QuestionId = questionId;
        Order = order;
        IsPresented = false;
    }

    /// <summary>
    /// Presents the question to participants with specified options.
    /// </summary>
    public void Present(
        bool showTitle = true,
        bool showOptionValues = true,
        int maxAttempts = 3,
        int? customDurationSeconds = null
    )
    {
        if (IsPresented && DeactivatedAt == null)
            throw new InvalidOperationException("Question is already being presented.");

        IsPresented = true;
        PresentedAt = DateTime.UtcNow;
        DeactivatedAt = null;
        ShowTitle = showTitle;
        ShowOptionValues = showOptionValues;
        MaxAttempts = maxAttempts;
        CustomDurationSeconds = customDurationSeconds;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the question (stops accepting answers).
    /// </summary>
    public void Deactivate()
    {
        if (!IsPresented)
            throw new InvalidOperationException("Question is not being presented.");

        DeactivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Deactivate all active attempts
        foreach (var attempt in Attempts.Where(a => a.IsActive))
        {
            attempt.Deactivate();
        }
    }

    /// <summary>
    /// Checks if the question is currently active (presented and not deactivated).
    /// </summary>
    public bool IsActive()
    {
        return IsPresented && DeactivatedAt == null;
    }

    /// <summary>
    /// Gets the effective duration for this question.
    /// </summary>
    public int? GetEffectiveDuration()
    {
        return CustomDurationSeconds;
    }

    /// <summary>
    /// Updates the order of this question.
    /// </summary>
    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
