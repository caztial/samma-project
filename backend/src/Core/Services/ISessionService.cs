using Core.Entities.Sessions;
using Core.Enums;

namespace Core.Services;

/// <summary>
/// Service interface for managing sessions.
/// </summary>
public interface ISessionService
{
    #region Session Management

    /// <summary>
    /// Creates a new session.
    /// </summary>
    Task<Session> CreateAsync(string name, string? location, string createdBy);

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    Task<Session?> GetByIdAsync(
        Guid id,
        bool includeParticipants = false,
        bool includeQuestions = false
    );

    /// <summary>
    /// Gets a session by its code.
    /// </summary>
    Task<Session?> GetByCodeAsync(string code);

    /// <summary>
    /// Gets all sessions with pagination and filtering.
    /// </summary>
    Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        SessionState? state = null,
        string? createdBy = null
    );

    /// <summary>
    /// Updates a session's details.
    /// </summary>
    Task<Session?> UpdateAsync(Guid id, string name, string? location);

    /// <summary>
    /// Deletes a session.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Activates a session (allows participants to join).
    /// </summary>
    Task<Session?> ActivateAsync(Guid sessionId);

    /// <summary>
    /// Deactivates (pauses) a session.
    /// </summary>
    Task<Session?> DeactivateAsync(Guid sessionId);

    /// <summary>
    /// Reactivates a paused session.
    /// </summary>
    Task<Session?> ReactivateAsync(Guid sessionId);

    /// <summary>
    /// Ends a session permanently.
    /// </summary>
    Task<Session?> EndAsync(Guid sessionId);

    #endregion

    #region Participant Management

    /// <summary>
    /// Participant joins a session using the session code.
    /// </summary>
    Task<SessionParticipant> JoinSessionAsync(string code, string userId);

    /// <summary>
    /// Participant leaves a session.
    /// </summary>
    Task<bool> LeaveSessionAsync(Guid sessionId, string userId);

    /// <summary>
    /// Gets all participants for a session.
    /// </summary>
    Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(Guid sessionId);

    /// <summary>
    /// Gets a specific participant.
    /// </summary>
    Task<SessionParticipant?> GetParticipantAsync(Guid sessionId, string userId);

    #endregion

    #region Question Assignment

    /// <summary>
    /// Assigns a question to a session.
    /// </summary>
    Task<SessionQuestion> AssignQuestionAsync(Guid sessionId, Guid questionId, int? order = null);

    /// <summary>
    /// Removes a question from a session.
    /// </summary>
    Task<bool> RemoveQuestionAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Gets all questions assigned to a session.
    /// </summary>
    Task<IEnumerable<SessionQuestion>> GetAssignedQuestionsAsync(Guid sessionId);

    /// <summary>
    /// Reorders questions in a session.
    /// </summary>
    Task<bool> ReorderQuestionsAsync(Guid sessionId, IEnumerable<Guid> questionIdsInOrder);

    #endregion

    #region Question Presentation

    /// <summary>
    /// Presents a question to participants.
    /// </summary>
    Task<SessionQuestion> PresentQuestionAsync(
        Guid sessionId,
        Guid questionId,
        bool showTitle = true,
        bool showOptionValues = true,
        int maxAttempts = 3,
        int? customDurationSeconds = null
    );

    /// <summary>
    /// Deactivates a presented question.
    /// </summary>
    Task<SessionQuestion?> DeactivateQuestionAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Gets currently presented questions with active attempts.
    /// </summary>
    Task<IEnumerable<SessionQuestion>> GetPresentedQuestionsAsync(Guid sessionId);

    /// <summary>
    /// Gets the active attempt for a presented question.
    /// </summary>
    Task<QuestionAttempt?> GetActiveAttemptAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Activates a specific attempt for a question.
    /// </summary>
    Task<QuestionAttempt> ActivateAttemptAsync(Guid sessionId, Guid questionId, int attemptNumber);

    /// <summary>
    /// Deactivates a specific attempt.
    /// </summary>
    Task<QuestionAttempt?> DeactivateAttemptAsync(
        Guid sessionId,
        Guid questionId,
        int attemptNumber
    );

    #endregion

    #region Answer Submission

    /// <summary>
    /// Submits an MCQ answer for a question attempt.
    /// </summary>
    Task<ParticipantMCQAnswer> SubmitMCQAnswerAsync(
        Guid sessionId,
        Guid questionId,
        string userId,
        int attemptNumber,
        Guid selectedOptionId
    );

    /// <summary>
    /// Gets all answers for a session question.
    /// </summary>
    Task<IEnumerable<ParticipantAnswer>> GetQuestionAnswersAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Gets a participant's answer for a specific attempt.
    /// </summary>
    Task<ParticipantAnswer?> GetParticipantAnswerAsync(
        Guid sessionId,
        Guid questionId,
        string userId,
        int attemptNumber
    );

    #endregion

    #region Scoring

    /// <summary>
    /// Calculates and returns scores for a session.
    /// </summary>
    Task<
        Dictionary<
            string,
            (string UserName, double TotalScore, int CorrectAnswers, int TotalAttempts)
        >
    > GetSessionScoresAsync(Guid sessionId);

    /// <summary>
    /// Calculates and returns scores for a specific question in a session.
    /// </summary>
    Task<
        Dictionary<string, (string UserName, double Score, bool IsCorrect, int AttemptNumber)>
    > GetQuestionScoresAsync(Guid sessionId, Guid questionId);

    /// <summary>
    /// Gets a participant's total score for a session.
    /// </summary>
    Task<double> GetParticipantTotalScoreAsync(Guid sessionId, string userId);

    #endregion
}
