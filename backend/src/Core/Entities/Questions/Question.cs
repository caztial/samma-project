using Core.Entities.Questions.ValueObjects;

namespace Core.Entities.Questions;

/// <summary>
/// Question Aggregate Root - Abstract base class for all question types.
/// Contains common elements shared across different question types.
/// </summary>
public abstract class Question : BaseEntity, IAggregatedRoot
{
    /// <summary>
    /// The question text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional description or explanation for the question
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Default time limit for answering this question in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Media attachments (audio/video) for this question
    /// </summary>
    public virtual ICollection<MediaMetadata> MediaMetadatas { get; set; } = [];

    /// <summary>
    /// Tags for searching and grouping questions (many-to-many via QuestionTag)
    /// </summary>
    public virtual ICollection<QuestionTag> QuestionTags { get; set; } = [];

    /// <summary>
    /// User ID who created this question
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Whether this question has been verified by an admin
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// The type of question (MCQ, TrueFalse, etc.)
    /// Set by derived classes in constructor.
    /// </summary>
    public string QuestionType { get; protected set; } = string.Empty;

    /// <summary>
    /// Adds media metadata to this question
    /// </summary>
    public void AddMedia(MediaMetadata media)
    {
        MediaMetadatas.Add(media);
    }
}
