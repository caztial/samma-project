using FastEndpoints;

namespace API.DTOs.Questions;

/// <summary>
/// Request DTO for creating a new MCQ question.
/// </summary>
public class CreateMCQQuestionRequest
{
    /// <summary>
    /// The question number/identifier (e.g., "Q1", "1.1", "A-001")
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// The question text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional description or explanation
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Default time limit in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Media attachments for the question
    /// </summary>
    public List<MediaMetadataDto>? MediaMetadatas { get; set; }

    /// <summary>
    /// Tags for the question
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Answer options (for MCQ - must have exactly one correct answer)
    /// </summary>
    public List<AnswerOptionDto>? AnswerOptions { get; set; }

    /// <summary>
    /// User ID from JWT claims
    /// </summary>
    [FromClaim("UserId")]
    public string? UserId { get; set; }
}

/// <summary>
/// Request DTO for updating an MCQ question.
/// </summary>
public class UpdateMCQQuestionRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Updated question number/identifier
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Updated question text
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Updated description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Updated duration in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Updated media attachments
    /// </summary>
    public List<MediaMetadataDto>? MediaMetadatas { get; set; }

    /// <summary>
    /// Admin only: verify/unverify question
    /// </summary>
    public bool? IsVerified { get; set; }
}

/// <summary>
/// Request DTO for adding a tag to a question.
/// </summary>
public class AddTagRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Tag name to add
    /// </summary>
    public string TagName { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for removing a tag from a question.
/// </summary>
public class RemoveTagRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Tag ID to remove
    /// </summary>
    public Guid TagId { get; set; }
}

/// <summary>
/// Request DTO for adding an answer option to an MCQ question.
/// </summary>
public class AddAnswerOptionRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// The answer option text
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
/// Request DTO for updating an answer option.
/// </summary>
public class UpdateAnswerOptionRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Answer option ID from route
    /// </summary>
    public Guid AnswerOptionId { get; set; }

    /// <summary>
    /// Updated text
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Updated order
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Updated points
    /// </summary>
    public int? Points { get; set; }

    /// <summary>
    /// Updated correctness flag
    /// </summary>
    public bool? IsCorrect { get; set; }
}

/// <summary>
/// Request DTO for deleting an answer option.
/// </summary>
public class DeleteAnswerOptionRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Answer option ID from route
    /// </summary>
    public Guid AnswerOptionId { get; set; }
}

/// <summary>
/// Request DTO for adding media to a question.
/// </summary>
public class AddMediaRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Type of media (Audio or Video)
    /// </summary>
    public string MediaType { get; set; } = string.Empty;

    /// <summary>
    /// URL to the media file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the media in seconds (optional)
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// MIME type of the media (e.g., "audio/mp3", "video/mp4")
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Thumbnail URL for video media (optional)
    /// </summary>
    public string? ThumbnailUrl { get; set; }
}

/// <summary>
/// Request DTO for updating media on a question.
/// </summary>
public class UpdateMediaRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Media ID from route
    /// </summary>
    public Guid MediaId { get; set; }

    /// <summary>
    /// Updated media type
    /// </summary>
    public string? MediaType { get; set; }

    /// <summary>
    /// Updated URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Updated duration in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Updated MIME type
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Updated thumbnail URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }
}

/// <summary>
/// Request DTO for deleting media from a question.
/// </summary>
public class DeleteMediaRequest
{
    /// <summary>
    /// Question ID from route
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Media ID from route
    /// </summary>
    public Guid MediaId { get; set; }
}

/// <summary>
/// Response DTO for a question.
/// </summary>
public class QuestionResponse
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public List<MediaMetadataDto> MediaMetadatas { get; set; } = [];
    public List<TagResponse> Tags { get; set; } = [];
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response DTO for an MCQ question (includes answer options).
/// </summary>
public class MCQQuestionResponse : QuestionResponse
{
    public List<AnswerOptionResponse> AnswerOptions { get; set; } = [];
}

/// <summary>
/// Response DTO for paginated question list.
/// </summary>
public class QuestionListResponse
{
    public List<QuestionResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>
/// Response DTO for paginated MCQ question list.
/// </summary>
public class MCQQuestionListResponse
{
    public List<MCQQuestionResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
