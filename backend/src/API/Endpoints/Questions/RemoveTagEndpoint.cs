using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to remove a tag from a question.
/// </summary>
public class RemoveTagEndpoint : Endpoint<RemoveTagRequest>
{
    private readonly ITagService _tagService;

    public RemoveTagEndpoint(ITagService tagService)
    {
        _tagService = tagService;
    }

    public override void Configure()
    {
        Delete("/questions/{QuestionId}/tags/{TagId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Remove a tag from a question";
            s.Description = "Removes a tag from a question. Only removes the association, the tag entity is preserved. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(RemoveTagRequest req, CancellationToken ct)
    {
        var success = await _tagService.RemoveTagFromQuestionAsync(req.QuestionId, req.TagId);

        if (!success)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question or tag association not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(new { message = "Tag removed successfully" }, cancellation: ct);
    }
}