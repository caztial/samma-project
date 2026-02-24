using API.DTOs.Questions;
using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Enums;
using FastEndpoints;

namespace API.Mappers;

/// <summary>
/// Mapper for MCQ Question entity.
/// </summary>
public class MCQQuestionMapper : Mapper<CreateMCQQuestionRequest, MCQQuestionResponse, McqQuestion>
{
    public override McqQuestion ToEntity(CreateMCQQuestionRequest r)
    {
        var question = new McqQuestion
        {
            Text = r.Text,
            Description = r.Description,
            DurationSeconds = r.DurationSeconds,
            CreatedBy = r.UserId ?? string.Empty,
            IsVerified = false
        };

        // Add media metadata
        if (r.MediaMetadatas != null)
        {
            foreach (var media in r.MediaMetadatas)
            {
                var mediaType = Enum.Parse<MediaType>(media.MediaType, ignoreCase: true);
                question.AddMedia(
                    new MediaMetadata(
                        mediaType,
                        media.Url,
                        media.DurationSeconds,
                        media.MimeType,
                        media.ThumbnailUrl
                    )
                );
            }
        }

        // Note: Tags are now handled via ITagService.AddTagToQuestionAsync after creation

        // Add answer options
        if (r.AnswerOptions != null)
        {
            foreach (var option in r.AnswerOptions)
            {
                question.AddAnswerOption(
                    option.Text,
                    option.Order,
                    option.Points,
                    option.IsCorrect
                );
            }
        }

        return question;
    }

    public override MCQQuestionResponse FromEntity(McqQuestion e)
    {
        return new MCQQuestionResponse
        {
            Id = e.Id,
            Text = e.Text,
            Description = e.Description,
            QuestionType = "MCQ",
            DurationSeconds = e.DurationSeconds,
            MediaMetadatas =
            [
                .. e.MediaMetadatas.Select(m => new MediaMetadataDto
                {
                    MediaType = m.MediaType.ToString(),
                    Url = m.Url,
                    DurationSeconds = m.DurationSeconds,
                    MimeType = m.MimeType,
                    ThumbnailUrl = m.ThumbnailUrl
                })
            ],
            Tags =
            [
                .. e
                    .QuestionTags.Where(qt => qt.Tag != null)
                    .Select(qt => new TagResponse { Id = qt.Tag!.Id, Name = qt.Tag!.Name })
            ],
            AnswerOptions =
            [
                .. e
                    .AnswerOptions.Select(o => new AnswerOptionResponse
                    {
                        Id = o.Id,
                        Text = o.Text,
                        Order = o.Order,
                        Points = o.Points,
                        IsCorrect = o.IsCorrect
                    })
                    .OrderBy(o => o.Order)
            ],
            CreatedBy = e.CreatedBy,
            IsVerified = e.IsVerified,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };
    }
}

/// <summary>
/// Response mapper for MCQ Question entity.
/// </summary>
public class MCQQuestionResponseMapper : ResponseMapper<MCQQuestionResponse, McqQuestion>
{
    public override MCQQuestionResponse FromEntity(McqQuestion e)
    {
        return new MCQQuestionResponse
        {
            Id = e.Id,
            Text = e.Text,
            Description = e.Description,
            QuestionType = "MCQ",
            DurationSeconds = e.DurationSeconds,
            MediaMetadatas =
            [
                .. e.MediaMetadatas.Select(m => new MediaMetadataDto
                {
                    MediaType = m.MediaType.ToString(),
                    Url = m.Url,
                    DurationSeconds = m.DurationSeconds,
                    MimeType = m.MimeType,
                    ThumbnailUrl = m.ThumbnailUrl
                })
            ],
            Tags =
            [
                .. e
                    .QuestionTags.Where(qt => qt.Tag != null)
                    .Select(qt => new TagResponse { Id = qt.Tag!.Id, Name = qt.Tag!.Name })
            ],
            AnswerOptions =
            [
                .. e
                    .AnswerOptions.Select(o => new AnswerOptionResponse
                    {
                        Id = o.Id,
                        Text = o.Text,
                        Order = o.Order,
                        Points = o.Points,
                        IsCorrect = o.IsCorrect
                    })
                    .OrderBy(o => o.Order)
            ],
            CreatedBy = e.CreatedBy,
            IsVerified = e.IsVerified,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };
    }
}
