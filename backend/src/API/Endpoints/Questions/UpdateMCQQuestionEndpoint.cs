using API.DTOs.Questions;
using API.Mappers;
using Core.Entities.Questions.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to update an MCQ question.
/// </summary>
public class UpdateMCQQuestionEndpoint
    : Endpoint<UpdateMCQQuestionRequest, MCQQuestionResponse, MCQQuestionResponseMapper>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public UpdateMCQQuestionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Put("/questions/mcq/{Id}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Update an MCQ question";
            s.Description = "Updates an MCQ question's basic properties. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(UpdateMCQQuestionRequest req, CancellationToken ct)
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

        var question = await _mcqQuestionService.UpdateAsync(
            req.Id,
            req.Number,
            req.Text,
            req.Description,
            req.DurationSeconds,
            mediaMetadatas,
            req.IsVerified
        );

        if (question == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "MCQ Question not found" },
                404,
                cancellation: ct
            );
            return;
        }

        Response = Map.FromEntity(question);
        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }
}