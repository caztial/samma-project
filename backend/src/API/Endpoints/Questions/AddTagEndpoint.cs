using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to add a tag to a question.
/// </summary>
public class AddTagEndpoint : Endpoint<AddTagRequest, TagResponse>
{
    private readonly ITagService _tagService;

    public AddTagEndpoint(ITagService tagService)
    {
        _tagService = tagService;
    }

    public override void Configure()
    {
        Post("/questions/{QuestionId}/tags");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Add a tag to a question";
            s.Description = "Adds a tag to a question by name. If the tag already exists, it will be reused. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(AddTagRequest req, CancellationToken ct)
    {
        var tag = await _tagService.AddTagToQuestionAsync(req.QuestionId, req.TagName);

        if (tag == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question not found" },
                404,
                cancellation: ct
            );
            return;
        }

        Response = new TagResponse { Id = tag.Id, Name = tag.Name };
        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }
}