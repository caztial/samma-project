using Core.Entities.Questions;

namespace Core.Services;

/// <summary>
/// Service interface for Tag operations.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Searches tags by name (typeahead). Returns top 10 results ordered by popularity.
    /// </summary>
    Task<IEnumerable<Tag>> SearchAsync(string searchText);

    /// <summary>
    /// Adds a tag to a question by tag name. Creates the tag if it doesn't exist.
    /// </summary>
    Task<Tag?> AddTagToQuestionAsync(Guid questionId, string tagName);

    /// <summary>
    /// Removes a tag from a question by tag ID.
    /// </summary>
    Task<bool> RemoveTagFromQuestionAsync(Guid questionId, Guid tagId);
}