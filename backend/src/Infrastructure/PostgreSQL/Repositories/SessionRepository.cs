using Core.Entities.Sessions;
using Core.Enums;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Repository implementation for Session aggregate.
/// </summary>
public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Session?> GetByIdAsync(
        Guid id,
        bool includeParticipants = false,
        bool includeQuestions = false
    )
    {
        var query = _context.Sessions.AsQueryable();

        if (includeParticipants)
        {
            query = query.Include(s => s.Participants).ThenInclude(p => p.User);
        }

        if (includeQuestions)
        {
            query = query.Include(s => s.SessionQuestions).ThenInclude(sq => sq.Question);
            query = query.Include(s => s.SessionQuestions).ThenInclude(sq => sq.Attempts);
        }

        return await query.AsSplitQuery().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Session?> GetByCodeAsync(string code)
    {
        return await _context.Sessions.FirstOrDefaultAsync(s =>
            s.Code == code && s.State == SessionState.Active
        );
    }

    public async Task<Session> CreateAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> UpdateAsync(Session session)
    {
        var existing = await _context.Sessions.FindAsync(session.Id);
        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(session);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sessions.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Sessions.AnyAsync(s => s.Code == code);
    }

    public async Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        SessionState? state = null,
        string? createdBy = null
    )
    {
        var query = _context.Sessions.AsQueryable();

        if (state.HasValue)
        {
            query = query.Where(s => s.State == state.Value);
        }

        if (!string.IsNullOrEmpty(createdBy))
        {
            query = query.Where(s => s.CreatedBy == createdBy);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsAsync()
    {
        return await _context
            .Sessions.Where(s => s.State == SessionState.Active)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Session>> GetSessionsByCreatorAsync(string userId)
    {
        return await _context
            .Sessions.Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<SessionParticipant?> GetParticipantAsync(Guid sessionId, string userId)
    {
        return await _context
            .SessionParticipants.Include(p => p.User)
            .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == userId);
    }

    public async Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(Guid sessionId)
    {
        return await _context
            .SessionParticipants.Include(p => p.User)
            .Where(p => p.SessionId == sessionId)
            .OrderBy(p => p.JoinedAt)
            .ToListAsync();
    }

    public async Task<SessionQuestion?> GetSessionQuestionAsync(Guid sessionId, Guid questionId)
    {
        return await _context
            .SessionQuestions.Include(sq => sq.Question)
            .Include(sq => sq.Attempts)
            .FirstOrDefaultAsync(sq => sq.SessionId == sessionId && sq.QuestionId == questionId);
    }

    public async Task<IEnumerable<SessionQuestion>> GetSessionQuestionsAsync(Guid sessionId)
    {
        return await _context
            .SessionQuestions.Include(sq => sq.Question)
            .ThenInclude(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .Where(sq => sq.SessionId == sessionId)
            .OrderBy(sq => sq.Order)
            .ToListAsync();
    }

    public async Task<IEnumerable<SessionQuestion>> GetPresentedQuestionsAsync(Guid sessionId)
    {
        return await _context
            .SessionQuestions.Include(sq => sq.Question)
            .Include(sq => sq.Attempts)
            .Where(sq => sq.SessionId == sessionId && sq.IsPresented && sq.DeactivatedAt == null)
            .OrderBy(sq => sq.Order)
            .ToListAsync();
    }

    public async Task<QuestionAttempt?> GetQuestionAttemptAsync(
        Guid sessionQuestionId,
        int attemptNumber
    )
    {
        return await _context.QuestionAttempts.FirstOrDefaultAsync(qa =>
            qa.SessionQuestionId == sessionQuestionId && qa.AttemptNumber == attemptNumber
        );
    }

    public async Task<IEnumerable<QuestionAttempt>> GetQuestionAttemptsAsync(Guid sessionQuestionId)
    {
        return await _context
            .QuestionAttempts.Where(qa => qa.SessionQuestionId == sessionQuestionId)
            .OrderBy(qa => qa.AttemptNumber)
            .ToListAsync();
    }

    public async Task<QuestionAttempt?> GetActiveAttemptAsync(Guid sessionQuestionId)
    {
        return await _context.QuestionAttempts.FirstOrDefaultAsync(qa =>
            qa.SessionQuestionId == sessionQuestionId && qa.IsActive
        );
    }

    public async Task<ParticipantAnswer?> GetParticipantAnswerAsync(
        Guid participantId,
        Guid attemptId
    )
    {
        return await _context.ParticipantAnswers.FirstOrDefaultAsync(pa =>
            pa.SessionParticipantId == participantId && pa.QuestionAttemptId == attemptId
        );
    }

    public async Task<IEnumerable<ParticipantAnswer>> GetSessionQuestionAnswersAsync(
        Guid sessionQuestionId
    )
    {
        return await _context
            .ParticipantAnswers.Include(pa => pa.Participant)
            .ThenInclude(p => p.User)
            .Include(pa => pa.Attempt)
            .Where(pa => pa.Attempt.SessionQuestionId == sessionQuestionId)
            .OrderByDescending(pa => pa.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParticipantAnswer>> GetParticipantAnswersAsync(Guid participantId)
    {
        return await _context
            .ParticipantAnswers.Include(pa => pa.Attempt)
            .ThenInclude(a => a.SessionQuestion)
            .ThenInclude(sq => sq.Question)
            .Where(pa => pa.SessionParticipantId == participantId)
            .OrderByDescending(pa => pa.SubmittedAt)
            .ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
