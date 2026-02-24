using API.DTOs.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to add media to a question.
/// </summary>
public class AddMediaEndpoint : Endpoint<AddMediaRequest, MediaMetadataDto>
{
    private readonly IQuestionService _questionService;

    public AddMediaEndpoint(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public override void Configure()
    {
        Post("/questions/{QuestionId}/media");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Add media to a question";
            s.Description = "Adds media (audio/video) to a question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(AddMediaRequest req, CancellationToken ct)
    {
        var mediaType = Enum.Parse<MediaType>(req.MediaType, ignoreCase: true);
        var media = new MediaMetadata(
            mediaType,
            req.Url,
            req.DurationSeconds,
            req.MimeType,
            req.ThumbnailUrl
        );

        var result = await _questionService.AddMediaAsync(req.QuestionId, media);

        if (result == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question not found" },
                404,
                cancellation: ct
            );
            return;
        }

        Response = new MediaMetadataDto
        {
            Id = result.Id,
            MediaType = result.MediaType.ToString(),
            Url = result.Url,
            DurationSeconds = result.DurationSeconds,
            MimeType = result.MimeType,
            ThumbnailUrl = result.ThumbnailUrl
        };

        await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
    }
}