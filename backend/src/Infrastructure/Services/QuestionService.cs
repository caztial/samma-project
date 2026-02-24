using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Enums;
using Core.Repositories;
using Core.Services;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for generic Question aggregate operations.
/// Read-only operations - create/update handled by concrete question type services.
/// </summary>
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _questionRepository.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<Question> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchText = null,
        IEnumerable<string>? tags = null,
        string? questionType = null
    )
    {
        return await _questionRepository.GetAllAsync(
            pageNumber,
            pageSize,
            searchText,
            tags,
            questionType
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _questionRepository.DeleteAsync(id);
    }

    public async Task<Tag?> AddTagAsync(Guid questionId, string tagName)
    {
        return await _questionRepository.AddTagAsync(questionId, tagName);
    }

    public async Task<bool> RemoveTagAsync(Guid questionId, Guid tagId)
    {
        return await _questionRepository.RemoveTagAsync(questionId, tagId);
    }

    public async Task<IEnumerable<string>> GetAllTagsAsync()
    {
        var (questions, _) = await _questionRepository.GetAllAsync(1, int.MaxValue);
        return questions
            .SelectMany(q => q.QuestionTags)
            .Select(qt => qt.Tag.Name)
            .Distinct()
            .OrderBy(t => t);
    }

    public async Task<IEnumerable<Question>> GetByTagAsync(string tagName)
    {
        return await _questionRepository.GetByTagAsync(tagName);
    }

    public async Task<MediaMetadata?> AddMediaAsync(Guid questionId, MediaMetadata media)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            return null;

        question.AddMedia(media);
        await _questionRepository.UpdateAsync(question);
        return media;
    }

    public async Task<MediaMetadata?> UpdateMediaAsync(
        Guid questionId,
        Guid mediaId,
        MediaType? mediaType = null,
        string? url = null,
        int? durationSeconds = null,
        string? mimeType = null,
        string? thumbnailUrl = null
    )
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            return null;

        var media = question.MediaMetadatas.FirstOrDefault(m => m.Id == mediaId);
        if (media == null)
            return null;

        // Since MediaMetadata is a value object, we need to create a new instance with updated values
        var updatedMedia = new MediaMetadata(
            id: media.Id,
            mediaType ?? media.MediaType,
            url ?? media.Url,
            durationSeconds ?? media.DurationSeconds,
            mimeType ?? media.MimeType,
            thumbnailUrl ?? media.ThumbnailUrl
        );

        // Remove old and add new
        question.MediaMetadatas.Remove(media);
        question.AddMedia(updatedMedia);

        await _questionRepository.UpdateAsync(question);
        return updatedMedia;
    }

    public async Task<bool> RemoveMediaAsync(Guid questionId, Guid mediaId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            return false;

        var media = question.MediaMetadatas.FirstOrDefault(m => m.Id == mediaId);
        if (media == null)
            return false;

        question.MediaMetadatas.Remove(media);
        await _questionRepository.UpdateAsync(question);
        return true;
    }
}
