namespace Core.Entities.Questions;

/// <summary>
/// Entity representing an answer option for an MCQ question.
/// Each option has text, display order, points, and correctness flag.
/// </summary>
public class McqAnswerOption : BaseEntity
{
    /// <summary>
    /// The MCQ question this option belongs to
    /// </summary>
    public Guid McqQuestionId { get; set; }

    /// <summary>
    /// Option number/identifier (e.g., "A", "B", "C", "1", "2", "3")
    /// </summary>
    public string OptionNumber { get; set; } = string.Empty;

    /// <summary>
    /// The text content of this answer option
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Display order for this option (0-based index)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Default points awarded for selecting this answer
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Whether this option is the correct answer
    /// For MCQ-Single, only one option should have this set to true
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Navigation to the parent MCQ question
    /// </summary>
    public McqQuestion? McqQuestion { get; set; }
}
