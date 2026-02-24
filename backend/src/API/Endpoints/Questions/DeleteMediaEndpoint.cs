using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to delete media from a question.
/// </summary>
public class DeleteMediaEndpoint : Endpoint<DeleteMediaRequest>
{
    private readonly IQuestionService _questionService;

    public DeleteMediaEndpoint(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public override void Configure()
    {
        Delete("/questions/{QuestionId}/media/{MediaId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Delete media from a question";
            s.Description = "Deletes media (audio/video) from a question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(DeleteMediaRequest req, CancellationToken ct)
    {
        var success = await _questionService.RemoveMediaAsync(req.QuestionId, req.MediaId);

        if (!success)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question or media not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Media deleted successfully" },
            cancellation: ct
        );
    }
}