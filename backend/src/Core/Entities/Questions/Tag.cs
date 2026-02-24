namespace Core.Entities.Questions;

/// <summary>
/// Entity representing a reusable tag for questions.
/// Tags can be shared across multiple questions via the QuestionTag join table.
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// Display name of the tag (e.g., "Dhamma", "Meditation", "Pali")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Normalized (lowercase) name for case-insensitive searching
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Number of questions using this tag
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Navigation to questions using this tag
    /// </summary>
    public ICollection<QuestionTag> QuestionTags { get; set; } = [];

    /// <summary>
    /// Creates a tag with normalized name automatically set
    /// </summary>
    public static Tag Create(string name)
    {
        return new Tag
        {
            Name = name,
            NormalizedName = name.ToLowerInvariant(),
            UsageCount = 0
        };
    }
}