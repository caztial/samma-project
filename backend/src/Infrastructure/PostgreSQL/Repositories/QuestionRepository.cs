using Core.Entities.Questions;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Repository implementation for Question aggregate.
/// </summary>
public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _context
            .Questions.Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .Include(q => q.MediaMetadatas)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<(IEnumerable<Question> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchText = null,
        IEnumerable<string>? tags = null,
        string? questionType = null
    )
    {
        var query = _context
            .Questions.Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .AsQueryable();

        // Apply question type filter
        if (!string.IsNullOrWhiteSpace(questionType))
        {
            query = query.Where(q => q.QuestionType == questionType);
        }

        // Apply text search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var searchLower = searchText.ToLower();
            query = query.Where(q => q.Text.ToLower().Contains(searchLower));
        }

        // Apply tags filter
        if (tags != null && tags.Any())
        {
            var normalizedTags = tags.Select(t => t.ToLowerInvariant()).ToList();
            query = query.Where(q =>
                q.QuestionTags.Any(qt => normalizedTags.Contains(qt.Tag.NormalizedName))
            );
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var question = await GetByIdAsync(id);
        if (question == null)
            return false;

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Questions.AnyAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Question>> GetByTagAsync(string tagName)
    {
        var normalizedTag = tagName.ToLowerInvariant();
        return await _context
            .Questions.Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .Where(q => q.QuestionTags.Any(qt => qt.Tag.NormalizedName == normalizedTag))
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByCreatorAsync(string userId)
    {
        return await _context
            .Questions.Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .Where(q => q.CreatedBy == userId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<Tag?> AddTagAsync(Guid questionId, string tagName)
    {
        // This method is now handled by TagService
        // Keeping for backward compatibility but should not be used
        throw new NotImplementedException("Use ITagService.AddTagToQuestionAsync instead");
    }

    public async Task<bool> RemoveTagAsync(Guid questionId, Guid tagId)
    {
        // This method is now handled by TagService
        // Keeping for backward compatibility but should not be used
        throw new NotImplementedException("Use ITagService.RemoveTagFromQuestionAsync instead");
    }

    public async Task<Question?> UpdateAsync(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
        return question;
    }
}