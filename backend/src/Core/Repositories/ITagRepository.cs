using Core.Entities.Questions;

namespace Core.Repositories;

/// <summary>
/// Repository interface for Tag entity operations.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Gets a tag by ID.
    /// </summary>
    Task<Tag?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a tag by its normalized name.
    /// </summary>
    Task<Tag?> GetByNameAsync(string normalizedName);

    /// <summary>
    /// Searches tags by name prefix (typeahead). Returns top 10 results.
    /// </summary>
    Task<IEnumerable<Tag>> SearchAsync(string searchText);

    /// <summary>
    /// Creates a new tag.
    /// </summary>
    Task<Tag> CreateAsync(Tag tag);

    /// <summary>
    /// Updates a tag.
    /// </summary>
    Task<Tag> UpdateAsync(Tag tag);

    /// <summary>
    /// Adds a tag to a question (creates QuestionTag association).
    /// </summary>
    Task<QuestionTag?> AddTagToQuestionAsync(Guid questionId, Guid tagId);

    /// <summary>
    /// Removes a tag from a question (removes QuestionTag association).
    /// </summary>
    Task<bool> RemoveTagFromQuestionAsync(Guid questionId, Guid tagId);

    /// <summary>
    /// Checks if a tag is already associated with a question.
    /// </summary>
    Task<bool> IsTagAssociatedAsync(Guid questionId, Guid tagId);
}