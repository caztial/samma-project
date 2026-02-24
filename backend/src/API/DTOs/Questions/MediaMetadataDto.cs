namespace API.DTOs.Questions;

/// <summary>
/// DTO for media metadata attached to a question.
/// </summary>
public class MediaMetadataDto
{
    /// <summary>
    /// Unique identifier for this media metadata
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of media: "Audio" or "Video"
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
    /// MIME type (e.g., "audio/mp3", "video/mp4")
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Thumbnail URL for video (optional)
    /// </summary>
    public string? ThumbnailUrl { get; set; }
}