namespace Core.Entities.Sessions;

/// <summary>
/// Represents an attempt for a question in a session.
/// Each attempt has a decreasing score multiplier.
/// </summary>
public class QuestionAttempt : BaseEntity
{
    /// <summary>
    /// The session question this attempt belongs to.
    /// </summary>
    public Guid SessionQuestionId { get; private set; }

    /// <summary>
    /// The attempt number (1, 2, 3, ...).
    /// </summary>
    public int AttemptNumber { get; private set; }

    /// <summary>
    /// Whether this attempt is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// When this attempt was activated.
    /// </summary>
    public DateTime? ActivatedAt { get; private set; }

    /// <summary>
    /// When this attempt was deactivated.
    /// </summary>
    public DateTime? DeactivatedAt { get; private set; }

    // Navigation properties
    public SessionQuestion SessionQuestion { get; private set; } = null!;

    /// <summary>
    /// Answers submitted during this attempt.
    /// </summary>
    public virtual ICollection<ParticipantAnswer> Answers { get; private set; } = [];

    // EF Core parameterless constructor
    private QuestionAttempt() { }

    /// <summary>
    /// Creates a new question attempt.
    /// </summary>
    public QuestionAttempt(Guid sessionQuestionId, int attemptNumber)
    {
        SessionQuestionId = sessionQuestionId;
        AttemptNumber = attemptNumber;
        IsActive = false;
    }

    /// <summary>
    /// Activates this attempt.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Attempt is already active.");

        IsActive = true;
        ActivatedAt = DateTime.UtcNow;
        DeactivatedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates this attempt.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Attempt is not active.");

        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the score multiplier for this attempt.
    /// Attempt 1: 100%, Attempt 2: 50%, Attempt 3: 25%, Attempt 4+: 0%
    /// </summary>
    public double GetScoreMultiplier()
    {
        return AttemptNumber switch
        {
            1 => 1.0, // 100%
            2 => 0.5, // 50%
            3 => 0.25, // 25%
            _ => 0.0 // 0% for attempts beyond 3
        };
    }

    /// <summary>
    /// Checks if this attempt is currently accepting answers.
    /// </summary>
    public bool IsAcceptingAnswers()
    {
        return IsActive && DeactivatedAt == null;
    }

    /// <summary>
    /// Checks if the allocated time has elapsed for this attempt.
    /// </summary>
    /// <param name="durationSeconds">The duration in seconds (if null, no timeout applies)</param>
    /// <param name="answeredAt">When the answer was submitted (request received time)</param>
    /// <returns>True if time has elapsed, false otherwise</returns>
    public bool HasTimeElapsed(int? durationSeconds, DateTimeOffset answeredAt)
    {
        if (!ActivatedAt.HasValue || !durationSeconds.HasValue)
            return false; // No duration = no timeout

        // Convert ActivatedAt to DateTimeOffset for comparison
        var activatedAtOffset = new DateTimeOffset(ActivatedAt.Value, TimeSpan.Zero);
        var elapsed = (answeredAt - activatedAtOffset).TotalSeconds;
        return elapsed > durationSeconds.Value;
    }
}
