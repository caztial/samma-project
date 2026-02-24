using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to get an MCQ question by ID.
/// </summary>
public class GetMcqQuestionEndpoint : EndpointWithoutRequest<MCQQuestionResponse>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public GetMcqQuestionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Get("/questions/mcq/{id}");
        Roles("Admin", "Moderator", "Presenter");
        Summary(s =>
        {
            s.Summary = "Get MCQ question by ID";
            s.Description =
                "Gets an MCQ question by ID including all answer options and tags. Requires Admin, Moderator, or Presenter role.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var question = await _mcqQuestionService.GetByIdAsync(id);

        if (question == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "MCQ Question not found" },
                404,
                cancellation: ct
            );
            return;
        }

        Response = new MCQQuestionResponse
        {
            Id = question.Id,
            Text = question.Text,
            Description = question.Description,
            QuestionType = "MCQ",
            DurationSeconds = question.DurationSeconds,
            MediaMetadatas =
            [
                .. question.MediaMetadatas.Select(m => new MediaMetadataDto
                {
                    Id = m.Id,
                    MediaType = m.MediaType.ToString(),
                    Url = m.Url,
                    DurationSeconds = m.DurationSeconds,
                    MimeType = m.MimeType,
                    ThumbnailUrl = m.ThumbnailUrl
                })
            ],
            Tags = question
                .QuestionTags.Select(qt => new TagResponse { Id = qt.Tag.Id, Name = qt.Tag.Name })
                .ToList(),
            AnswerOptions =
            [
                .. question
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
            CreatedBy = question.CreatedBy,
            IsVerified = question.IsVerified,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };

        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }
}
