using Core.Enums;

namespace Core.Entities.Questions.ValueObjects;

/// <summary>
/// Value object representing media metadata (audio/video) attached to a question.
/// </summary>
public sealed class MediaMetadata
{
    /// <summary>
    /// Unique identifier for this media metadata
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Type of media (Audio or Video)
    /// </summary>
    public MediaType MediaType { get; init; }

    /// <summary>
    /// URL to the media file
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// Duration of the media in seconds (optional)
    /// </summary>
    public int? DurationSeconds { get; init; }

    /// <summary>
    /// MIME type of the media (e.g., "audio/mp3", "video/mp4")
    /// </summary>
    public string? MimeType { get; init; }

    /// <summary>
    /// Thumbnail URL for video media (optional)
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    public MediaMetadata() { }

    public MediaMetadata(
        MediaType mediaType,
        string url,
        int? durationSeconds = null,
        string? mimeType = null,
        string? thumbnailUrl = null
    )
    {
        Id = Guid.NewGuid();
        MediaType = mediaType;
        Url = url;
        DurationSeconds = durationSeconds;
        MimeType = mimeType;
        ThumbnailUrl = thumbnailUrl;
    }

    public MediaMetadata(
        Guid id,
        MediaType mediaType,
        string url,
        int? durationSeconds = null,
        string? mimeType = null,
        string? thumbnailUrl = null
    )
    {
        Id = id;
        MediaType = mediaType;
        Url = url;
        DurationSeconds = durationSeconds;
        MimeType = mimeType;
        ThumbnailUrl = thumbnailUrl;
    }

    /// <summary>
    /// Empty instance for default values
    /// </summary>
    public static MediaMetadata Empty => new();
}
