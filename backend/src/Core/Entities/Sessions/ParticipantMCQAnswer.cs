namespace Core.Entities.Sessions;

/// <summary>
/// Represents an MCQ answer submitted by a participant.
/// </summary>
public class ParticipantMCQAnswer : ParticipantAnswer
{
    /// <summary>
    /// The ID of the selected MCQ option.
    /// </summary>
    public Guid SelectedOptionId { get; private set; }

    // EF Core parameterless constructor
    private ParticipantMCQAnswer() { }

    /// <summary>
    /// Creates a new MCQ answer.
    /// </summary>
    public ParticipantMCQAnswer(
        Guid sessionParticipantId,
        Guid questionAttemptId,
        Guid questionId,
        Guid selectedOptionId,
        bool isCorrect,
        int basePoints,
        double finalScore
    )
        : base(
            sessionParticipantId,
            questionAttemptId,
            questionId,
            isCorrect,
            basePoints,
            finalScore,
            "MCQ"
        )
    {
        SelectedOptionId = selectedOptionId;
        ValidateAnswer();
    }

    /// <summary>
    /// Validates the MCQ answer data.
    /// </summary>
    public override void ValidateAnswer()
    {
        if (SelectedOptionId == Guid.Empty)
            throw new InvalidOperationException("SelectedOptionId must be a valid GUID.");
    }
}
