using Core.Entities.Questions;

namespace Core.Entities.Sessions;

/// <summary>
/// Abstract base class for participant answers.
/// Different question types will have different answer implementations.
/// </summary>
public abstract class ParticipantAnswer : BaseEntity
{
    /// <summary>
    /// The participant who submitted this answer.
    /// </summary>
    public Guid SessionParticipantId { get; private set; }

    /// <summary>
    /// The question attempt this answer belongs to.
    /// </summary>
    public Guid QuestionAttemptId { get; private set; }

    /// <summary>
    /// The question being answered.
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Whether the answer is correct.
    /// </summary>
    public bool IsCorrect { get; protected set; }

    /// <summary>
    /// The base points for this question (before multiplier).
    /// </summary>
    public int BasePoints { get; private set; }

    /// <summary>
    /// The final score after applying attempt multiplier.
    /// </summary>
    public double FinalScore { get; private set; }

    /// <summary>
    /// When the answer was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// The type of answer (e.g., "MCQ", "TrueFalse", "ShortAnswer").
    /// </summary>
    public string AnswerType { get; protected set; } = string.Empty;

    // Navigation properties
    public SessionParticipant Participant { get; private set; } = null!;
    public QuestionAttempt Attempt { get; private set; } = null!;
    public Question Question { get; private set; } = null!;

    // EF Core parameterless constructor
    protected ParticipantAnswer() { }

    /// <summary>
    /// Creates a new participant answer.
    /// </summary>
    protected ParticipantAnswer(
        Guid sessionParticipantId,
        Guid questionAttemptId,
        Guid questionId,
        bool isCorrect,
        int basePoints,
        double finalScore,
        string answerType
    )
    {
        SessionParticipantId = sessionParticipantId;
        QuestionAttemptId = questionAttemptId;
        QuestionId = questionId;
        IsCorrect = isCorrect;
        BasePoints = basePoints;
        FinalScore = finalScore;
        SubmittedAt = DateTime.UtcNow;
        AnswerType = answerType;
    }

    /// <summary>
    /// Validates the answer-specific data.
    /// </summary>
    public abstract void ValidateAnswer();
}