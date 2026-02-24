using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;

namespace Core.Services;

/// <summary>
/// Service interface for generic Question aggregate operations.
/// Read-only operations - create/update handled by concrete question type services.
/// </summary>
public interface IQuestionService
{
    /// <summary>
    /// Gets a question by ID.
    /// </summary>
    Task<Question?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all questions with pagination and optional filtering.
    /// </summary>
    Task<(IEnumerable<Question> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchText = null,
        IEnumerable<string>? tags = null,
        string? questionType = null
    );

    /// <summary>
    /// Deletes a question.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Adds a tag to a question.
    /// </summary>
    Task<Tag?> AddTagAsync(Guid questionId, string tagName);

    /// <summary>
    /// Removes a tag from a question.
    /// </summary>
    Task<bool> RemoveTagAsync(Guid questionId, Guid tagId);

    /// <summary>
    /// Gets all unique tag names in the system.
    /// </summary>
    Task<IEnumerable<string>> GetAllTagsAsync();

    /// <summary>
    /// Gets questions by tag.
    /// </summary>
    Task<IEnumerable<Question>> GetByTagAsync(string tagName);

    /// <summary>
    /// Adds media to a question.
    /// </summary>
    Task<MediaMetadata?> AddMediaAsync(Guid questionId, MediaMetadata media);

    /// <summary>
    /// Updates media on a question.
    /// </summary>
    Task<MediaMetadata?> UpdateMediaAsync(
        Guid questionId,
        Guid mediaId,
        Core.Enums.MediaType? mediaType = null,
        string? url = null,
        int? durationSeconds = null,
        string? mimeType = null,
        string? thumbnailUrl = null
    );

    /// <summary>
    /// Removes media from a question.
    /// </summary>
    Task<bool> RemoveMediaAsync(Guid questionId, Guid mediaId);
}
