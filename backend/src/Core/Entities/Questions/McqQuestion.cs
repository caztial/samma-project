namespace Core.Entities.Questions;

/// <summary>
/// MCQ (Multiple Choice Question) - A question type with multiple answer options.
/// Supports single correct answer validation.
/// </summary>
public class McqQuestion : Question
{
    /// <summary>
    /// Answer options for this MCQ question
    /// </summary>
    public virtual ICollection<McqAnswerOption> AnswerOptions { get; set; } = [];

    public McqQuestion()
    {
        QuestionType = "MCQ";
    }

    /// <summary>
    /// Adds an answer option to this MCQ question
    /// </summary>
    /// <param name="text">The option text</param>
    /// <param name="order">Display order of the option</param>
    /// <param name="points">Points awarded for selecting this option</param>
    /// <param name="isCorrect">Whether this option is the correct answer</param>
    /// <param name="optionNumber">Option number/identifier (e.g., "A", "B", "C")</param>
    public McqAnswerOption AddAnswerOption(
        string text,
        int order,
        int points,
        bool isCorrect,
        string optionNumber = ""
    )
    {
        var option = new McqAnswerOption
        {
            McqQuestionId = Id,
            OptionNumber = optionNumber,
            Text = text,
            Order = order,
            Points = points,
            IsCorrect = isCorrect
        };
        AnswerOptions.Add(option);
        return option;
    }

    /// <summary>
    /// Validates that this MCQ question has exactly one correct answer
    /// </summary>
    /// <returns>True if exactly one correct answer exists, false otherwise</returns>
    public bool ValidateMCQ()
    {
        if (AnswerOptions == null || AnswerOptions.Count == 0)
            return true;
        var correctCount = AnswerOptions.Count(o => o.IsCorrect);
        return correctCount == 1;
    }
}
