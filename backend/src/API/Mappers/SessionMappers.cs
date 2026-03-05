using API.DTOs.Sessions;
using Core.Entities.Questions;
using Core.Entities.Sessions;
using FastEndpoints;

namespace API.Mappers;

/// <summary>
/// Mapper for Session entity to SessionResponse.
/// </summary>
public class SessionMapper : ResponseMapper<SessionResponse, Session>
{
    public override SessionResponse FromEntity(Session session)
    {
        return new SessionResponse
        {
            Id = session.Id,
            Name = session.Name,
            Code = session.Code,
            Location = session.Location,
            State = session.State.ToString(),
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            CreatedAt = session.CreatedAt
        };
    }
}

/// <summary>
/// Mapper for SessionParticipant entity to SessionParticipantResponse.
/// </summary>
public class SessionParticipantMapper
    : ResponseMapper<SessionParticipantResponse, SessionParticipant>
{
    public override SessionParticipantResponse FromEntity(SessionParticipant participant)
    {
        return new SessionParticipantResponse
        {
            Id = participant.Id,
            SessionId = participant.SessionId,
            UserId = participant.UserId,
            UserName =
                participant.User != null
                    ? (participant.User.UserName ?? participant.User.Email ?? participant.UserId)
                    : participant.UserId,
            JoinedAt = participant.JoinedAt,
            LeftAt = participant.LeftAt
        };
    }
}

/// <summary>
/// Mapper for SessionQuestion entity to SessionQuestionResponse.
/// </summary>
public class SessionQuestionMapper : ResponseMapper<SessionQuestionResponse, SessionQuestion>
{
    public override SessionQuestionResponse FromEntity(SessionQuestion sq)
    {
        return new SessionQuestionResponse
        {
            Id = sq.Id,
            QuestionId = sq.QuestionId,
            QuestionNumber = sq.Question?.Number ?? string.Empty,
            QuestionText = sq.ShowTitle ? sq.Question?.Text : null,
            Order = sq.Order,
            IsPresented = sq.IsPresented,
            PresentedAt = sq.PresentedAt,
            DeactivatedAt = sq.DeactivatedAt
        };
    }
}

/// <summary>
/// Mapper for SessionQuestion entity to PresentedQuestionResponse.
/// </summary>
public class PresentedQuestionMapper : ResponseMapper<PresentedQuestionResponse, SessionQuestion>
{
    public override PresentedQuestionResponse FromEntity(SessionQuestion sq)
    {
        var activeAttempt = sq.Attempts.FirstOrDefault(a => a.IsActive);

        return new PresentedQuestionResponse
        {
            SessionQuestionId = sq.Id,
            QuestionId = sq.QuestionId,
            QuestionNumber = sq.Question?.Number ?? string.Empty,
            QuestionText = sq.ShowTitle ? sq.Question?.Text : null,
            ShowTitle = sq.ShowTitle,
            ShowOptionValues = sq.ShowOptionValues,
            MaxAttempts = sq.MaxAttempts,
            DurationSeconds = sq.CustomDurationSeconds ?? sq.Question?.DurationSeconds,
            PresentedAt = sq.PresentedAt,
            ActiveAttempt =
                activeAttempt != null
                    ? new ActiveAttemptResponse
                    {
                        AttemptNumber = activeAttempt.AttemptNumber,
                        ScoreMultiplier = activeAttempt.GetScoreMultiplier(),
                        ActivatedAt = activeAttempt.ActivatedAt
                    }
                    : null,
            AvailableAttempts =
            [
                .. Enumerable
                    .Range(1, sq.MaxAttempts)
                    .Select(i => new AvailableAttemptResponse
                    {
                        AttemptNumber = i,
                        IsActive =
                            sq.Attempts.FirstOrDefault(a => a.AttemptNumber == i)?.IsActive
                            ?? false,
                        ScoreMultiplier = i switch
                        {
                            1 => 1.0,
                            2 => 0.5,
                            3 => 0.25,
                            _ => 0.0
                        }
                    })
            ]
        };
    }
}

/// <summary>
/// Mapper for SessionQuestion entity to PresentedQuestionResponse.
/// </summary>
public class PresentedMcqQuestionMapper
    : ResponseMapper<PresentedMcqQuestionResponse, SessionQuestion>
{
    public override PresentedMcqQuestionResponse FromEntity(SessionQuestion sq)
    {
        var activeAttempt = sq.Attempts.FirstOrDefault(a => a.IsActive);
        var question = (McqQuestion)sq.Question;

        return new PresentedMcqQuestionResponse
        {
            SessionQuestionId = sq.Id,
            QuestionId = sq.QuestionId,
            QuestionNumber = sq.Question?.Number ?? string.Empty,
            QuestionText = sq.ShowTitle ? sq.Question?.Text : null,
            QuestionDescription = sq.ShowTitle ? sq.Question?.Description : null,
            Options =
            [
                .. question
                    .AnswerOptions.OrderBy(o => o.Order)
                    .Select(o => new PresentedMcqAnswerResponse
                    {
                        OptionId = o.Id,
                        OptionNumber = o.OptionNumber,
                        OptionText = sq.ShowOptionValues ? o.Text : string.Empty,
                        Order = o.Order
                    })
            ],
            ShowTitle = sq.ShowTitle,
            ShowOptionValues = sq.ShowOptionValues,
            MaxAttempts = sq.MaxAttempts,
            DurationSeconds = sq.CustomDurationSeconds ?? sq.Question?.DurationSeconds,
            PresentedAt = sq.PresentedAt,
            ActiveAttempt =
                activeAttempt != null
                    ? new ActiveAttemptResponse
                    {
                        AttemptNumber = activeAttempt.AttemptNumber,
                        ScoreMultiplier = activeAttempt.GetScoreMultiplier(),
                        ActivatedAt = activeAttempt.ActivatedAt
                    }
                    : null,
            AvailableAttempts =
            [
                .. Enumerable
                    .Range(1, sq.MaxAttempts)
                    .Select(i => new AvailableAttemptResponse
                    {
                        AttemptNumber = i,
                        IsActive =
                            sq.Attempts.FirstOrDefault(a => a.AttemptNumber == i)?.IsActive
                            ?? false,
                        ScoreMultiplier = i switch
                        {
                            1 => 1.0,
                            2 => 0.5,
                            3 => 0.25,
                            _ => 0.0
                        }
                    })
            ]
        };
    }
}

/// <summary>
/// Mapper for ParticipantMCQAnswer entity to SubmitAnswerResponse.
/// </summary>
public class SubmitAnswerMapper : ResponseMapper<SubmitAnswerResponse, ParticipantMCQAnswer>
{
    public override SubmitAnswerResponse FromEntity(ParticipantMCQAnswer answer)
    {
        return new SubmitAnswerResponse
        {
            AnswerId = answer.Id,
            QuestionId = answer.QuestionId,
            AttemptNumber = answer.Attempt?.AttemptNumber ?? 0,
            SelectedOptionId = answer.SelectedOptionId,
            IsCorrect = answer.IsCorrect,
            BasePoints = answer.BasePoints,
            FinalScore = answer.FinalScore
        };
    }
}
