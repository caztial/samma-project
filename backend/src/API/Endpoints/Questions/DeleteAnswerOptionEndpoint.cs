using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to delete an answer option.
/// </summary>
public class DeleteAnswerOptionEndpoint : Endpoint<DeleteAnswerOptionRequest>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public DeleteAnswerOptionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Delete("/questions/mcq/{QuestionId}/answers/{AnswerOptionId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Delete an answer option";
            s.Description =
                "Deletes an answer option from an MCQ question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(DeleteAnswerOptionRequest req, CancellationToken ct)
    {
        try
        {
            var success = await _mcqQuestionService.RemoveAnswerOptionAsync(
                req.QuestionId,
                req.AnswerOptionId
            );

            if (!success)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "Answer option not found" },
                    404,
                    cancellation: ct
                );
                return;
            }

            await HttpContext.Response.SendAsync(
                new { message = "Answer option deleted successfully" },
                cancellation: ct
            );
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}
