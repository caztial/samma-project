using Core.Entities.Questions;
using Core.Repositories;
using Core.Services;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for Tag operations.
/// </summary>
public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IQuestionRepository _questionRepository;

    public TagService(ITagRepository tagRepository, IQuestionRepository questionRepository)
    {
        _tagRepository = tagRepository;
        _questionRepository = questionRepository;
    }

    public async Task<IEnumerable<Tag>> SearchAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Enumerable.Empty<Tag>();
        }

        return await _tagRepository.SearchAsync(searchText);
    }

    public async Task<Tag?> AddTagToQuestionAsync(Guid questionId, string tagName)
    {
        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            return null;

        var normalizedName = tagName.ToLowerInvariant().Trim();

        // Find existing tag or create new
        var tag = await _tagRepository.GetByNameAsync(normalizedName);

        if (tag == null)
        {
            // Create new tag
            tag = Tag.Create(tagName);
            tag = await _tagRepository.CreateAsync(tag);
        }

        // Check if already associated
        var isAssociated = await _tagRepository.IsTagAssociatedAsync(questionId, tag.Id);
        if (isAssociated)
        {
            return tag;
        }

        // Add association
        await _tagRepository.AddTagToQuestionAsync(questionId, tag.Id);

        return tag;
    }

    public async Task<bool> RemoveTagFromQuestionAsync(Guid questionId, Guid tagId)
    {
        return await _tagRepository.RemoveTagFromQuestionAsync(questionId, tagId);
    }
}