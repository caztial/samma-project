namespace Core.Entities.Questions;

/// <summary>
/// Join entity for the Question-Tag many-to-many relationship.
/// </summary>
public class QuestionTag : BaseEntity
{
    /// <summary>
    /// The question this tag belongs to
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// The tag this question belongs to
    /// </summary>
    public Guid TagId { get; set; }

    /// <summary>
    /// Navigation to the question
    /// </summary>
    public Question? Question { get; set; }

    /// <summary>
    /// Navigation to the tag
    /// </summary>
    public Tag? Tag { get; set; }
}