namespace API.DTOs.Questions;

/// <summary>
/// DTO for an answer option.
/// </summary>
public class AnswerOptionDto
{
    /// <summary>
    /// Option text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Display order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Points for this answer
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Whether this is the correct answer
    /// </summary>
    public bool IsCorrect { get; set; }
}

/// <summary>
/// Response DTO for an answer option (includes ID).
/// </summary>
public class AnswerOptionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public int Points { get; set; }
    public bool IsCorrect { get; set; }
}