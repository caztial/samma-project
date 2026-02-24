using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Request for deleting a question.
/// </summary>
public class DeleteQuestionRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint to delete a question by ID.
/// </summary>
public class DeleteQuestionEndpoint : Endpoint<DeleteQuestionRequest>
{
    private readonly IQuestionService _questionService;

    public DeleteQuestionEndpoint(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public override void Configure()
    {
        Delete("/questions/{Id}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Delete a question";
            s.Description = "Deletes a question by ID. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(DeleteQuestionRequest req, CancellationToken ct)
    {
        var success = await _questionService.DeleteAsync(req.Id);

        if (!success)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Question not found" },
                404,
                cancellation: ct
            );
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Question deleted successfully" },
            cancellation: ct
        );
    }
}