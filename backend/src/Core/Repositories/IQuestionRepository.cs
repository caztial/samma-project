using Core.Entities.Questions;

namespace Core.Repositories;

/// <summary>
/// Repository interface for Question aggregate.
/// Read-only operations - create/update handled by concrete question type repositories.
/// </summary>
public interface IQuestionRepository
{
    /// <summary>
    /// Gets a question by ID including tags.
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
    /// Deletes a question by ID.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a question exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Gets questions by tag name.
    /// </summary>
    Task<IEnumerable<Question>> GetByTagAsync(string tagName);

    /// <summary>
    /// Gets questions created by a specific user.
    /// </summary>
    Task<IEnumerable<Question>> GetByCreatorAsync(string userId);

    /// <summary>
    /// Adds a tag to a question and persists the change.
    /// </summary>
    Task<Tag?> AddTagAsync(Guid questionId, string tagName);

    /// <summary>
    /// Removes a tag from a question and persists the change.
    /// </summary>
    Task<bool> RemoveTagAsync(Guid questionId, Guid tagId);

    /// <summary>
    /// Updates a question and persists the change.
    /// </summary>
    Task<Question?> UpdateAsync(Question question);
}
