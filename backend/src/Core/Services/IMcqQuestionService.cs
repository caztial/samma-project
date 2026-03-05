using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;

namespace Core.Services;

/// <summary>
/// Service interface for MCQ Question specific operations.
/// </summary>
public interface IMcqQuestionService
{
    /// <summary>
    /// Creates a new MCQ question.
    /// </summary>
    Task<McqQuestion> CreateAsync(
        string number,
        string text,
        string? description,
        int? durationSeconds,
        string createdBy,
        IEnumerable<MediaMetadata>? mediaMetadatas = null,
        IEnumerable<string>? tags = null,
        IEnumerable<(string Text, int Order, int Points, bool IsCorrect)>? answerOptions = null
    );

    /// <summary>
    /// Gets an MCQ question by ID.
    /// </summary>
    Task<McqQuestion?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an MCQ question's basic properties.
    /// </summary>
    Task<McqQuestion?> UpdateAsync(
        Guid id,
        string? number = null,
        string? text = null,
        string? description = null,
        int? durationSeconds = null,
        IEnumerable<MediaMetadata>? mediaMetadatas = null,
        bool? isVerified = null
    );

    /// <summary>
    /// Adds an answer option to an MCQ question.
    /// </summary>
    Task<McqAnswerOption?> AddAnswerOptionAsync(
        Guid questionId,
        string text,
        int order,
        int points,
        bool isCorrect,
        string optionNumber = ""
    );

    /// <summary>
    /// Removes an answer option from an MCQ question.
    /// </summary>
    Task<bool> RemoveAnswerOptionAsync(Guid questionId, Guid answerOptionId);

    /// <summary>
    /// Updates an answer option.
    /// </summary>
    Task<McqAnswerOption?> UpdateAnswerOptionAsync(
        Guid questionId,
        Guid answerOptionId,
        string? text = null,
        int? order = null,
        int? points = null,
        bool? isCorrect = null,
        string? optionNumber = null
    );
}
