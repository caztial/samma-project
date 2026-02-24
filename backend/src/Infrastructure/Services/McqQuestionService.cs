using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Repositories;
using Core.Services;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for MCQ Question specific operations.
/// </summary>
public class McqQuestionService : IMcqQuestionService
{
    private readonly IMcqQuestionRepository _mcqQuestionRepository;

    public McqQuestionService(IMcqQuestionRepository mcqQuestionRepository)
    {
        _mcqQuestionRepository = mcqQuestionRepository;
    }

    public async Task<McqQuestion> CreateAsync(
        string text,
        string? description,
        int? durationSeconds,
        string createdBy,
        IEnumerable<MediaMetadata>? mediaMetadatas = null,
        IEnumerable<string>? tags = null,
        IEnumerable<(string Text, int Order, int Points, bool IsCorrect)>? answerOptions = null
    )
    {
        var question = new McqQuestion
        {
            Text = text,
            Description = description,
            DurationSeconds = durationSeconds,
            CreatedBy = createdBy,
            IsVerified = false
        };

        // Add media metadata
        if (mediaMetadatas != null)
        {
            foreach (var media in mediaMetadatas)
            {
                question.AddMedia(media);
            }
        }

        // Note: Tags are now handled via ITagService.AddTagToQuestionAsync after creation

        // Add answer options
        if (answerOptions != null)
        {
            foreach (var option in answerOptions)
            {
                question.AddAnswerOption(
                    option.Text,
                    option.Order,
                    option.Points,
                    option.IsCorrect
                );
            }
        }

        // Validate MCQ
        if (!question.ValidateMCQ())
        {
            throw new InvalidOperationException(
                "MCQ questions must have exactly one correct answer."
            );
        }

        return await _mcqQuestionRepository.CreateAsync(question);
    }

    public async Task<McqQuestion?> GetByIdAsync(Guid id)
    {
        return await _mcqQuestionRepository.GetByIdAsync(id);
    }

    public async Task<McqQuestion?> UpdateAsync(
        Guid id,
        string? text = null,
        string? description = null,
        int? durationSeconds = null,
        IEnumerable<MediaMetadata>? mediaMetadatas = null,
        bool? isVerified = null
    )
    {
        var question = await _mcqQuestionRepository.GetByIdAsync(id);
        if (question == null)
            return null;

        if (text != null)
            question.Text = text;

        if (description != null)
            question.Description = description;

        if (durationSeconds != null)
            question.DurationSeconds = durationSeconds;

        if (mediaMetadatas != null)
        {
            question.MediaMetadatas.Clear();
            foreach (var media in mediaMetadatas)
            {
                question.AddMedia(media);
            }
        }

        if (isVerified != null)
            question.IsVerified = isVerified.Value;

        return await _mcqQuestionRepository.UpdateAsync(question);
    }

    public async Task<McqAnswerOption?> AddAnswerOptionAsync(
        Guid questionId,
        string text,
        int order,
        int points,
        bool isCorrect
    )
    {
        var question = await _mcqQuestionRepository.GetByIdAsync(questionId);
        if (question == null)
            return null;

        // For MCQ, ensure only one correct answer
        if (isCorrect)
        {
            // Reset all other options to not correct
            foreach (var option in question.AnswerOptions)
            {
                option.IsCorrect = false;
            }
        }

        var answerOption = question.AddAnswerOption(text, order, points, isCorrect);

        // Validate and save
        if (!question.ValidateMCQ())
        {
            throw new InvalidOperationException(
                "MCQ questions must have exactly one correct answer."
            );
        }
        await _mcqQuestionRepository.AddAnswerOptionAsync(answerOption);
        await _mcqQuestionRepository.UpdateAsync(question);
        return question.AnswerOptions.FirstOrDefault(o => o.Text == text && o.Order == order);
    }

    public async Task<bool> RemoveAnswerOptionAsync(Guid questionId, Guid answerOptionId)
    {
        var question = await _mcqQuestionRepository.GetByIdAsync(questionId);
        if (question == null)
            return false;

        var option = question.AnswerOptions.FirstOrDefault(o => o.Id == answerOptionId);
        if (option == null)
            return false;

        question.AnswerOptions.Remove(option);

        // Validate and save
        if (!question.ValidateMCQ())
        {
            throw new InvalidOperationException(
                "Cannot remove the only correct answer from MCQ question."
            );
        }

        await _mcqQuestionRepository.UpdateAsync(question);
        return true;
    }

    public async Task<McqAnswerOption?> UpdateAnswerOptionAsync(
        Guid questionId,
        Guid answerOptionId,
        string? text = null,
        int? order = null,
        int? points = null,
        bool? isCorrect = null
    )
    {
        var question = await _mcqQuestionRepository.GetByIdAsync(questionId);
        if (question == null)
            return null;

        var option = question.AnswerOptions.FirstOrDefault(o => o.Id == answerOptionId);
        if (option == null)
            return null;

        // For MCQ, ensure only one correct answer
        if (isCorrect == true)
        {
            // Reset all other options to not correct
            foreach (var opt in question.AnswerOptions)
            {
                opt.IsCorrect = false;
            }
        }

        if (text != null)
            option.Text = text;

        if (order != null)
            option.Order = order.Value;

        if (points != null)
            option.Points = points.Value;

        if (isCorrect != null)
            option.IsCorrect = isCorrect.Value;

        // Validate and save
        if (!question.ValidateMCQ())
        {
            throw new InvalidOperationException(
                "MCQ questions must have exactly one correct answer."
            );
        }

        await _mcqQuestionRepository.UpdateAsync(question);
        return option;
    }
}
