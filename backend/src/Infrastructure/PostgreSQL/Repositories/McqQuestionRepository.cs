using Core.Entities.Questions;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Repository implementation for MCQ Question specific operations.
/// </summary>
public class McqQuestionRepository : IMcqQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public McqQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<McqQuestion?> GetByIdAsync(Guid id)
    {
        return await _context
            .MCQQuestions.Include(q => q.AnswerOptions)
            .AsTracking()
            .Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .AsSplitQuery() // Avoid cartesian explosion when including multiple collections
            .AsTracking()
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<McqQuestion> CreateAsync(McqQuestion question)
    {
        _context.MCQQuestions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<McqQuestion?> UpdateAsync(McqQuestion question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        //_context.MCQQuestions.Update(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<McqAnswerOption?> GetAnswerOptionAsync(Guid questionId, Guid answerOptionId)
    {
        return await _context.AnswerOptions.FirstOrDefaultAsync(o =>
            o.Id == answerOptionId && o.McqQuestionId == questionId
        );
    }

    public async Task<McqAnswerOption> AddAnswerOptionAsync(McqAnswerOption option)
    {
        _context.AnswerOptions.Add(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task<McqAnswerOption?> UpdateAnswerOptionAsync(McqAnswerOption option)
    {
        option.UpdatedAt = DateTime.UtcNow;
        //_context.AnswerOptions.Update(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task<bool> RemoveAnswerOptionAsync(Guid questionId, Guid answerOptionId)
    {
        var option = await GetAnswerOptionAsync(questionId, answerOptionId);
        if (option == null)
            return false;

        _context.AnswerOptions.Remove(option);
        await _context.SaveChangesAsync();
        return true;
    }
}
