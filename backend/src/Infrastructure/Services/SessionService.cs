using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Entities.Sessions;
using Core.Enums;
using Core.Events;
using Core.Repositories;
using Core.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for managing sessions.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMcqQuestionRepository _mcqQuestionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        ISessionRepository sessionRepository,
        IQuestionRepository questionRepository,
        IMcqQuestionRepository mcqQuestionRepository,
        IUserRepository userRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<SessionService> logger
    )
    {
        _sessionRepository = sessionRepository;
        _questionRepository = questionRepository;
        _mcqQuestionRepository = mcqQuestionRepository;
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    #region Session Management

    public async Task<Session> CreateAsync(string name, string? location, string createdBy)
    {
        // Generate a unique session code
        string code = GenerateSessionCode();

        var session = new Session(name, code, location, createdBy);

        var createdSession = await _sessionRepository.CreateAsync(session);

        _logger.LogInformation(
            "Session created: {SessionId}, Code: {Code}",
            createdSession.Id,
            createdSession.Code
        );

        return createdSession;
    }

    public async Task<Session?> GetByIdAsync(
        Guid id,
        bool includeParticipants = false,
        bool includeQuestions = false
    )
    {
        return await _sessionRepository.GetByIdAsync(id, includeParticipants, includeQuestions);
    }

    public async Task<Session?> GetByCodeAsync(string code)
    {
        return await _sessionRepository.GetByCodeAsync(code);
    }

    public async Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        SessionState? state = null,
        string? createdBy = null
    )
    {
        return await _sessionRepository.GetAllAsync(pageNumber, pageSize, state, createdBy);
    }

    public async Task<Session?> UpdateAsync(Guid id, string name, string? location)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
            return null;

        session.Update(name, location);
        return await _sessionRepository.UpdateAsync(session);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
            return false;

        if (session.State == SessionState.Active)
            throw new InvalidOperationException("Cannot delete an active session.");

        return await _sessionRepository.DeleteAsync(id);
    }

    public async Task<Session?> ActivateAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        session.Activate();
        var updatedSession = await _sessionRepository.UpdateAsync(session);

        _logger.LogInformation("Session activated: {SessionId}", session.Id);

        return updatedSession;
    }

    public async Task<Session?> DeactivateAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        session.Deactivate();
        return await _sessionRepository.UpdateAsync(session);
    }

    public async Task<Session?> ReactivateAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        session.Reactivate();
        return await _sessionRepository.UpdateAsync(session);
    }

    public async Task<Session?> EndAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        session.End();
        var updatedSession = await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new SessionEndedEvent { SessionId = session.Id, Code = session.Code }
        );

        _logger.LogInformation("Session ended: {SessionId}", session.Id);

        return updatedSession;
    }

    #endregion

    #region Participant Management

    public async Task<SessionParticipant> JoinSessionAsync(string code, string userId)
    {
        var session = await _sessionRepository.GetByCodeAsync(code);
        if (session == null)
            throw new InvalidOperationException("Session not found.");

        if (!session.CanJoin())
            throw new InvalidOperationException("Session is not accepting participants.");

        // Check if already joined
        var existingParticipant = await _sessionRepository.GetParticipantAsync(session.Id, userId);
        if (existingParticipant != null)
        {
            // If they previously left, they can rejoin
            if (existingParticipant.LeftAt != null)
            {
                throw new InvalidOperationException("Participant has already left this session.");
            }
            return existingParticipant;
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var participant = new SessionParticipant(session.Id, userId);

        // Add participant to session
        session.Participants.Add(participant);
        await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new ParticipantJoinedEvent
            {
                SessionId = session.Id,
                Code = session.Code,
                UserId = userId,
                UserName = user.UserName ?? user.Email ?? userId,
                JoinedAt = participant.JoinedAt
            }
        );

        _logger.LogInformation(
            "Participant joined session: {SessionId}, UserId: {UserId}",
            session.Id,
            userId
        );

        return participant;
    }

    public async Task<bool> LeaveSessionAsync(Guid sessionId, string userId)
    {
        var participant = await _sessionRepository.GetParticipantAsync(sessionId, userId);
        if (participant == null)
            return false;

        if (participant.LeftAt != null)
            return false; // Already left

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return false;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        participant.Leave();
        await _sessionRepository.SaveChangesAsync();

        // Publish event
        await _publishEndpoint.Publish(
            new ParticipantLeftEvent
            {
                SessionId = sessionId,
                Code = session.Code,
                UserId = userId,
                UserName = user.UserName ?? user.Email ?? userId,
                LeftAt = participant.LeftAt!.Value
            }
        );

        _logger.LogInformation(
            "Participant left session: {SessionId}, UserId: {UserId}",
            sessionId,
            userId
        );

        return true;
    }

    public async Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(Guid sessionId)
    {
        return await _sessionRepository.GetParticipantsAsync(sessionId);
    }

    public async Task<SessionParticipant?> GetParticipantAsync(Guid sessionId, string userId)
    {
        return await _sessionRepository.GetParticipantAsync(sessionId, userId);
    }

    #endregion

    #region Question Assignment

    public async Task<SessionQuestion> AssignQuestionAsync(
        Guid sessionId,
        Guid questionId,
        int? order = null
    )
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, includeQuestions: true);
        if (session == null)
            throw new InvalidOperationException("Session not found.");

        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            throw new InvalidOperationException("Question not found.");

        // Check if already assigned
        var existing = session.SessionQuestions.Any(sq => sq.QuestionId == questionId);
        if (existing)
            throw new InvalidOperationException("Question is already assigned to this session.");

        // Determine order
        var questions = session.SessionQuestions;
        var newOrder = order ?? (questions.Any() ? questions.Max(q => q.Order) + 1 : 1);

        var sessionQuestion = new SessionQuestion(sessionId, questionId, newOrder);

        session.SessionQuestions.Add(sessionQuestion);
        await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new QuestionAssignedToSessionEvent { SessionId = sessionId, QuestionId = questionId }
        );

        _logger.LogInformation(
            "Question assigned to session: {SessionId}, QuestionId: {QuestionId}",
            sessionId,
            questionId
        );

        return sessionQuestion;
    }

    public async Task<bool> RemoveQuestionAsync(Guid sessionId, Guid questionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, includeQuestions: true);
        if (session == null)
            return false;

        var sessionQuestion = session.SessionQuestions.FirstOrDefault(sq =>
            sq.QuestionId == questionId
        );
        if (sessionQuestion == null)
            return false;

        if (sessionQuestion.IsPresented && sessionQuestion.Attempts.Any(a => a.Answers.Any()))
            throw new InvalidOperationException(
                "Cannot remove a question that is currently being presented or have answers."
            );

        session.SessionQuestions.Remove(sessionQuestion);
        await _sessionRepository.UpdateAsync(session);

        return true;
    }

    public async Task<IEnumerable<SessionQuestion>> GetAssignedQuestionsAsync(Guid sessionId)
    {
        return await _sessionRepository.GetSessionQuestionsAsync(sessionId);
    }

    public async Task<bool> ReorderQuestionsAsync(
        Guid sessionId,
        IEnumerable<Guid> questionIdsInOrder
    )
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, includeQuestions: true);
        if (session == null)
            return false;

        var questionIdList = questionIdsInOrder.ToList();
        for (int i = 0; i < questionIdList.Count; i++)
        {
            var sessionQuestion = session.SessionQuestions.FirstOrDefault(sq =>
                sq.QuestionId == questionIdList[i]
            );
            if (sessionQuestion != null)
            {
                sessionQuestion.UpdateOrder(i + 1);
            }
        }

        await _sessionRepository.UpdateAsync(session);
        return true;
    }

    #endregion

    #region Question Presentation

    public async Task<SessionQuestion> PresentQuestionAsync(
        Guid sessionId,
        Guid questionId,
        bool showTitle = true,
        bool showOptionValues = true,
        int maxAttempts = 3,
        int? customDurationSeconds = null
    )
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new InvalidOperationException("Session not found.");

        if (session.State != SessionState.Active)
            throw new InvalidOperationException("Session must be active to present questions.");

        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            throw new InvalidOperationException("Question is not assigned to this session.");

        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            throw new InvalidOperationException("Question not found.");

        // Present the question
        sessionQuestion.Present(showTitle, showOptionValues, maxAttempts, customDurationSeconds);

        // Create the first attempt and activate it
        var firstAttempt = new QuestionAttempt(sessionQuestion.Id, 1);
        firstAttempt.Activate();
        sessionQuestion.Attempts.Add(firstAttempt);

        await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new QuestionPresentedEvent
            {
                SessionId = sessionId,
                Code = session.Code,
                QuestionId = questionId,
                SessionQuestionId = sessionQuestion.Id,
                ShowTitle = showTitle,
                ShowOptionValues = showOptionValues,
                MaxAttempts = maxAttempts,
                DurationSeconds = customDurationSeconds ?? question.DurationSeconds
            }
        );

        _logger.LogInformation(
            "Question presented: SessionId: {SessionId}, QuestionId: {QuestionId}",
            sessionId,
            questionId
        );

        return sessionQuestion;
    }

    public async Task<SessionQuestion?> DeactivateQuestionAsync(Guid sessionId, Guid questionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null || !sessionQuestion.IsActive())
            return null;

        sessionQuestion.Deactivate();
        await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new QuestionDeactivatedEvent
            {
                SessionId = sessionId,
                Code = session.Code,
                QuestionId = questionId,
                SessionQuestionId = sessionQuestion.Id
            }
        );

        _logger.LogInformation(
            "Question deactivated: SessionId: {SessionId}, QuestionId: {QuestionId}",
            sessionId,
            questionId
        );

        return sessionQuestion;
    }

    public async Task<IEnumerable<SessionQuestion>> GetPresentedQuestionsAsync(Guid sessionId)
    {
        var sessionQuestions = await _sessionRepository.GetPresentedQuestionsAsync(sessionId);
        foreach (var sq in sessionQuestions)
        {
            if (sq.Question is McqQuestion mcqQuestion)
            {
                sq.Question =
                    await _mcqQuestionRepository.GetByIdAsync(mcqQuestion.Id) ?? sq.Question;
            }
        }

        return sessionQuestions;
    }

    public async Task<QuestionAttempt?> GetActiveAttemptAsync(Guid sessionId, Guid questionId)
    {
        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            return null;

        return await _sessionRepository.GetActiveAttemptAsync(sessionQuestion.Id);
    }

    public async Task<QuestionAttempt> ActivateAttemptAsync(
        Guid sessionId,
        Guid questionId,
        int attemptNumber
    )
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new InvalidOperationException("Session not found.");

        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            throw new InvalidOperationException("Question is not assigned to this session.");

        if (!sessionQuestion.IsActive())
            throw new InvalidOperationException("Question is not currently active.");

        if (attemptNumber > sessionQuestion.MaxAttempts)
            throw new InvalidOperationException(
                $"Maximum attempts ({sessionQuestion.MaxAttempts}) exceeded."
            );

        // Deactivate any currently active attempts
        var activeAttempt = await _sessionRepository.GetActiveAttemptAsync(sessionQuestion.Id);
        if (activeAttempt != null)
        {
            activeAttempt.Deactivate();
        }

        // Check if attempt exists, create if not
        var attempt = await _sessionRepository.GetQuestionAttemptAsync(
            sessionQuestion.Id,
            attemptNumber
        );
        if (attempt == null)
        {
            attempt = new QuestionAttempt(sessionQuestion.Id, attemptNumber);
            sessionQuestion.Attempts.Add(attempt);
        }

        attempt.Activate();
        await _sessionRepository.UpdateAsync(session);

        // Publish event
        await _publishEndpoint.Publish(
            new QuestionAttemptActivatedEvent
            {
                SessionId = sessionId,
                Code = session.Code,
                QuestionId = questionId,
                SessionQuestionId = sessionQuestion.Id,
                AttemptNumber = attemptNumber,
                ScoreMultiplier = attempt.GetScoreMultiplier()
            }
        );

        _logger.LogInformation(
            "Attempt activated: SessionId: {SessionId}, QuestionId: {QuestionId}, Attempt: {AttemptNumber}",
            sessionId,
            questionId,
            attemptNumber
        );

        return attempt;
    }

    public async Task<QuestionAttempt?> DeactivateAttemptAsync(
        Guid sessionId,
        Guid questionId,
        int attemptNumber
    )
    {
        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            return null;

        var attempt = await _sessionRepository.GetQuestionAttemptAsync(
            sessionQuestion.Id,
            attemptNumber
        );
        if (attempt == null || !attempt.IsActive)
            return null;

        attempt.Deactivate();
        await _sessionRepository.SaveChangesAsync();

        return attempt;
    }

    #endregion

    #region Answer Submission

    public async Task<ParticipantMCQAnswer> SubmitMCQAnswerAsync(
        Guid sessionId,
        Guid questionId,
        string userId,
        int attemptNumber,
        Guid selectedOptionId,
        DateTimeOffset answeredAt
    )
    {
        // Validate session
        var session = await _sessionRepository.GetByIdAsync(sessionId, true, true);
        if (session == null)
            throw new InvalidOperationException("Session not found.");

        if (session.State != SessionState.Active)
            throw new InvalidOperationException("Session is not active.");

        // Validate question is active
        var sessionQuestion = session.SessionQuestions.FirstOrDefault(sq =>
            sq.QuestionId == questionId
        );

        if (sessionQuestion == null || !sessionQuestion.IsActive())
            throw new InvalidOperationException("Question is not currently active.");

        // Validate attempt is active
        var attempt = sessionQuestion.Attempts.FirstOrDefault(a =>
            a.AttemptNumber == attemptNumber
        );

        if (attempt == null || !attempt.IsAcceptingAnswers())
            throw new InvalidOperationException(
                $"Attempt {attemptNumber} is not accepting answers."
            );

        // Get question for duration and answer validation
        var question = await _mcqQuestionRepository.GetByIdAsync(questionId);
        if (question == null)
            throw new InvalidOperationException("Question not found.");

        // Check if time has elapsed for this attempt using request received time
        var effectiveDuration = sessionQuestion.GetEffectiveDuration(question.DurationSeconds);
        if (attempt.HasTimeElapsed(effectiveDuration, answeredAt))
            throw new InvalidOperationException(
                "Time has elapsed for this attempt. Answers are no longer accepted."
            );

        // Get participant
        var participant = session.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
            throw new InvalidOperationException("Participant not found in session.");

        if (participant.LeftAt != null)
            throw new InvalidOperationException("Participant has left the session.");

        // Check if already answered this attempt
        var existingAnswer = await _sessionRepository.GetParticipantAnswerAsync(
            participant.Id,
            attempt.Id
        );
        if (existingAnswer != null)
            throw new InvalidOperationException("Already submitted an answer for this attempt.");

        // Validate MCQ question
        if (question is not McqQuestion mcqQuestion)
            throw new InvalidOperationException("Question is not an MCQ question.");

        // Find the selected option
        var selectedOption = mcqQuestion.AnswerOptions.FirstOrDefault(o =>
            o.Id == selectedOptionId
        );
        if (selectedOption == null)
            throw new InvalidOperationException("Invalid option selected.");

        // Calculate score
        var isCorrect = selectedOption.IsCorrect;
        var basePoints = selectedOption.Points;
        var finalScore = isCorrect ? basePoints * attempt.GetScoreMultiplier() : 0;

        // Create the answer
        var answer = new ParticipantMCQAnswer(
            participant.Id,
            attempt.Id,
            questionId,
            selectedOptionId,
            isCorrect,
            basePoints,
            finalScore
        );

        participant.Answers.Add(answer);
        await _sessionRepository.SaveChangesAsync();

        // Get user for event
        var user = await _userRepository.GetByIdAsync(userId);

        // Publish event
        await _publishEndpoint.Publish(
            new AnswerSubmittedEvent
            {
                SessionId = sessionId,
                Code = session.Code,
                QuestionId = questionId,
                SessionQuestionId = sessionQuestion.Id,
                AnswerId = answer.Id,
                UserId = userId,
                UserName = user != null ? (user.UserName ?? user.Email ?? userId) : userId,
                AttemptNumber = attemptNumber,
                IsCorrect = isCorrect,
                Score = finalScore
            }
        );

        _logger.LogInformation(
            "Answer submitted: SessionId: {SessionId}, QuestionId: {QuestionId}, UserId: {UserId}, Attempt: {Attempt}, Score: {Score}",
            sessionId,
            questionId,
            userId,
            attemptNumber,
            finalScore
        );

        return answer;
    }

    public async Task<IEnumerable<ParticipantAnswer>> GetQuestionAnswersAsync(
        Guid sessionId,
        Guid questionId
    )
    {
        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            return [];

        return await _sessionRepository.GetSessionQuestionAnswersAsync(sessionQuestion.Id);
    }

    public async Task<ParticipantAnswer?> GetParticipantAnswerAsync(
        Guid sessionId,
        Guid questionId,
        string userId,
        int attemptNumber
    )
    {
        var participant = await _sessionRepository.GetParticipantAsync(sessionId, userId);
        if (participant == null)
            return null;

        var sessionQuestion = await _sessionRepository.GetSessionQuestionAsync(
            sessionId,
            questionId
        );
        if (sessionQuestion == null)
            return null;

        var attempt = await _sessionRepository.GetQuestionAttemptAsync(
            sessionQuestion.Id,
            attemptNumber
        );
        if (attempt == null)
            return null;

        return await _sessionRepository.GetParticipantAnswerAsync(participant.Id, attempt.Id);
    }

    #endregion

    #region Scoring

    public async Task<
        Dictionary<
            string,
            (string UserName, double TotalScore, int CorrectAnswers, int TotalAttempts)
        >
    > GetSessionScoresAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, includeParticipants: true);
        if (session == null)
            return [];

        var scores =
            new Dictionary<
                string,
                (string UserName, double TotalScore, int CorrectAnswers, int TotalAttempts)
            >();

        foreach (var participant in session.Participants)
        {
            var answers = await _sessionRepository.GetParticipantAnswersAsync(participant.Id);
            var answerList = answers.ToList();

            var totalScore = answerList.Sum(a => a.FinalScore);
            var correctAnswers = answerList.Count(a => a.IsCorrect);
            var totalAttempts = answerList.Count;

            var userName =
                participant.User != null
                    ? (participant.User.UserName ?? participant.User.Email ?? participant.UserId)
                    : participant.UserId;

            scores[participant.UserId] = (userName, totalScore, correctAnswers, totalAttempts);
        }

        return scores;
    }

    public async Task<
        Dictionary<string, (string UserName, double Score, bool IsCorrect, int AttemptNumber)>
    > GetQuestionScoresAsync(Guid sessionId, Guid questionId)
    {
        var answers = await GetQuestionAnswersAsync(sessionId, questionId);
        var scores =
            new Dictionary<
                string,
                (string UserName, double Score, bool IsCorrect, int AttemptNumber)
            >();

        // Group by participant and get their best answer (highest score)
        foreach (var answer in answers)
        {
            var participant = answer.Participant;
            var userId = participant.UserId;

            // If user already has a better score, skip
            if (scores.ContainsKey(userId))
            {
                var existing = scores[userId];
                if (existing.Score >= answer.FinalScore)
                    continue;
            }

            var userName =
                participant.User != null
                    ? (participant.User.UserName ?? participant.User.Email ?? userId)
                    : userId;

            scores[userId] = (
                userName,
                answer.FinalScore,
                answer.IsCorrect,
                answer.Attempt?.AttemptNumber ?? 0
            );
        }

        return scores;
    }

    public async Task<double> GetParticipantTotalScoreAsync(Guid sessionId, string userId)
    {
        var participant = await _sessionRepository.GetParticipantAsync(sessionId, userId);
        if (participant == null)
            return 0;

        var answers = await _sessionRepository.GetParticipantAnswersAsync(participant.Id);
        return answers.Sum(a => a.FinalScore);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates a random session code in the format "XXXX-XXXX-XXXX".
    /// </summary>
    private static string GenerateSessionCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        var random = Random.Shared;
        var segments = new string[3];

        for (int i = 0; i < 3; i++)
        {
            var segment = new char[4];
            for (int j = 0; j < 4; j++)
            {
                segment[j] = chars[random.Next(chars.Length)];
            }
            segments[i] = new string(segment);
        }

        return string.Join("-", segments);
    }

    #endregion
}
