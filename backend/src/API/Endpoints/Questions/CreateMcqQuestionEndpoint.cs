using API.DTOs.Questions;
using API.Mappers;
using Core.Entities.Questions.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to create a new MCQ question.
/// </summary>
public class CreateMcqQuestionEndpoint
    : Endpoint<CreateMCQQuestionRequest, MCQQuestionResponse, MCQQuestionMapper>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public CreateMcqQuestionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Post("/questions/mcq");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Create a new MCQ question";
            s.Description =
                "Creates a new Multiple Choice Question in the question bank. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(CreateMCQQuestionRequest req, CancellationToken ct)
    {
        // Parse media metadata
        IEnumerable<MediaMetadata>? mediaMetadatas = null;
        if (req.MediaMetadatas != null)
        {
            mediaMetadatas = req.MediaMetadatas.Select(m =>
            {
                var mediaType = Enum.Parse<MediaType>(m.MediaType, ignoreCase: true);
                return new MediaMetadata(
                    mediaType,
                    m.Url,
                    m.DurationSeconds,
                    m.MimeType,
                    m.ThumbnailUrl
                );
            });
        }

        // Parse answer options
        IEnumerable<(string Text, int Order, int Points, bool IsCorrect)>? answerOptions = null;
        if (req.AnswerOptions != null)
        {
            answerOptions = req.AnswerOptions.Select(o => (o.Text, o.Order, o.Points, o.IsCorrect));
        }

        try
        {
            var question = await _mcqQuestionService.CreateAsync(
                req.Number,
                req.Text,
                req.Description,
                req.DurationSeconds,
                req.UserId!,
                mediaMetadatas,
                req.Tags,
                answerOptions
            );

            Response = Map.FromEntity(question);
            await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}
