using API.DTOs.Questions;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to update media on a question.
/// </summary>
public class UpdateMediaEndpoint : Endpoint<UpdateMediaRequest, MediaMetadataDto>
{
    private readonly IQuestionService _questionService;

    public UpdateMediaEndpoint(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public override void Configure()
    {
        Put("/questions/{QuestionId}/media/{MediaId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Update media on a question";
            s.Description = "Updates media (audio/video) on a question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(UpdateMediaRequest req, CancellationToken ct)
    {
        MediaType? mediaType = null;
        if (!string.IsNullOrEmpty(req.MediaType))
        {
            mediaType = Enum.Parse<MediaType>(req.MediaType, ignoreCase: true);
        }

        var result = await _questionService.UpdateMediaAsync(
            req.QuestionId,
            req.MediaId,
            mediaType,
            req.Url,
            req.DurationSeconds,
            req.MimeType,
            req.ThumbnailUrl
        );

        if (result == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question or media not found" },
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

        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }
}