using Core.Entities.Questions;

namespace Core.Repositories;

/// <summary>
/// Repository interface for MCQ Question specific operations.
/// </summary>
public interface IMcqQuestionRepository
{
    /// <summary>
    /// Gets an MCQ question by ID including answer options and tags.
    /// </summary>
    Task<McqQuestion?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new MCQ question.
    /// </summary>
    Task<McqQuestion> CreateAsync(McqQuestion question);

    /// <summary>
    /// Updates an existing MCQ question.
    /// </summary>
    Task<McqQuestion?> UpdateAsync(McqQuestion question);

    /// <summary>
    /// Gets an answer option by ID.
    /// </summary>
    Task<McqAnswerOption?> GetAnswerOptionAsync(Guid questionId, Guid answerOptionId);

    /// <summary>
    /// Adds an answer option to an MCQ question.
    /// </summary>
    Task<McqAnswerOption> AddAnswerOptionAsync(McqAnswerOption option);

    /// <summary>
    /// Updates an answer option.
    /// </summary>
    Task<McqAnswerOption?> UpdateAnswerOptionAsync(McqAnswerOption option);

    /// <summary>
    /// Removes an answer option.
    /// </summary>
    Task<bool> RemoveAnswerOptionAsync(Guid questionId, Guid answerOptionId);
}
