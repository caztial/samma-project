using Core.Entities.Questions;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Repository implementation for Tag entity operations.
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<Tag?> GetByNameAsync(string normalizedName)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.NormalizedName == normalizedName);
    }

    public async Task<IEnumerable<Tag>> SearchAsync(string searchText)
    {
        var normalizedSearch = searchText.ToLowerInvariant();
        return await _context
            .Tags.Where(t => t.NormalizedName.Contains(normalizedSearch))
            .OrderByDescending(t => t.UsageCount)
            .ThenBy(t => t.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<QuestionTag?> AddTagToQuestionAsync(Guid questionId, Guid tagId)
    {
        // Check if already associated
        var existing = await _context.QuestionTags.FirstOrDefaultAsync(qt =>
            qt.QuestionId == questionId && qt.TagId == tagId
        );

        if (existing != null)
            return existing;

        var questionTag = new QuestionTag { QuestionId = questionId, TagId = tagId };

        _context.QuestionTags.Add(questionTag);

        // Increment usage count
        var tag = await _context.Tags.FindAsync(tagId);
        if (tag != null)
        {
            tag.UsageCount++;
        }

        await _context.SaveChangesAsync();
        return questionTag;
    }

    public async Task<bool> RemoveTagFromQuestionAsync(Guid questionId, Guid tagId)
    {
        var questionTag = await _context.QuestionTags.FirstOrDefaultAsync(qt =>
            qt.QuestionId == questionId && qt.TagId == tagId
        );

        if (questionTag == null)
            return false;

        _context.QuestionTags.Remove(questionTag);

        // Decrement usage count
        var tag = await _context.Tags.FindAsync(tagId);
        if (tag != null && tag.UsageCount > 0)
        {
            tag.UsageCount--;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsTagAssociatedAsync(Guid questionId, Guid tagId)
    {
        return await _context.QuestionTags.AnyAsync(qt =>
            qt.QuestionId == questionId && qt.TagId == tagId
        );
    }
}