using Core.Entities.Sessions;
using Core.Enums;

namespace Core.Repositories;

/// <summary>
/// Repository interface for Session aggregate.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Gets a session by ID including related data.
    /// </summary>
    Task<Session?> GetByIdAsync(
        Guid id,
        bool includeParticipants = false,
        bool includeQuestions = false
    );

    /// <summary>
    /// Gets a session by its unique code.
    /// </summary>
    Task<Session?> GetByCodeAsync(string code);

    /// <summary>
    /// Creates a new session.
    /// </summary>
    Task<Session> CreateAsync(Session session);

    /// <summary>
    /// Updates an existing session.
    /// </summary>
    Task<Session?> UpdateAsync(Session session);

    /// <summary>
    /// Deletes a session by ID.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a session exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Checks if a code is already in use.
    /// </summary>
    Task<bool> CodeExistsAsync(string code);

    /// <summary>
    /// Gets all sessions with optional filtering.
    /// </summary>
    Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        SessionState? state = null,
        string? createdBy = null
    );

    /// <summary>
    /// Gets all active sessions.
    /// </summary>
    Task<IEnumerable<Session>> GetActiveSessionsAsync();

    /// <summary>
    /// Gets sessions created by a specific user.
    /// </summary>
    Task<IEnumerable<Session>> GetSessionsByCreatorAsync(string userId);

    /// <summary>
    /// Gets a session participant by ID.
    /// </summary>
    Task<SessionParticipant?> GetParticipantAsync(Guid sessionId, string userId);

    /// <summary>
    /// Gets all participants for a session.
    /// </summary>
    Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(Guid sessionId);

    /// <summary>
    /// Gets a session question by ID.
    /// </summary>
    Task<SessionQuestion?> GetSessionQuestionAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Gets all questions assigned to a session.
    /// </summary>
    Task<IEnumerable<SessionQuestion>> GetSessionQuestionsAsync(Guid sessionId);

    /// <summary>
    /// Gets currently presented questions for a session.
    /// </summary>
    Task<IEnumerable<SessionQuestion>> GetPresentedQuestionsAsync(Guid sessionId);

    /// <summary>
    /// Gets a question attempt by ID.
    /// </summary>
    Task<QuestionAttempt?> GetQuestionAttemptAsync(Guid sessionQuestionId, int attemptNumber);

    /// <summary>
    /// Gets all attempts for a session question.
    /// </summary>
    Task<IEnumerable<QuestionAttempt>> GetQuestionAttemptsAsync(Guid sessionQuestionId);

    /// <summary>
    /// Gets the active attempt for a session question.
    /// </summary>
    Task<QuestionAttempt?> GetActiveAttemptAsync(Guid sessionQuestionId);

    /// <summary>
    /// Gets a participant's answer for a specific attempt.
    /// </summary>
    Task<ParticipantAnswer?> GetParticipantAnswerAsync(Guid participantId, Guid attemptId);

    /// <summary>
    /// Gets all answers for a session question.
    /// </summary>
    Task<IEnumerable<ParticipantAnswer>> GetSessionQuestionAnswersAsync(Guid sessionQuestionId);

    /// <summary>
    /// Gets all answers for a participant in a session.
    /// </summary>
    Task<IEnumerable<ParticipantAnswer>> GetParticipantAnswersAsync(Guid participantId);

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync();
}
