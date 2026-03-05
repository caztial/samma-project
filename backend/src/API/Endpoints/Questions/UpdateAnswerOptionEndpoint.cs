using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to update an answer option.
/// </summary>
public class UpdateAnswerOptionEndpoint : Endpoint<UpdateAnswerOptionRequest, AnswerOptionResponse>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public UpdateAnswerOptionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Put("/questions/mcq/{QuestionId}/answers/{AnswerOptionId}");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Update an answer option";
            s.Description =
                "Updates an answer option in an MCQ question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(UpdateAnswerOptionRequest req, CancellationToken ct)
    {
        try
        {
            var option = await _mcqQuestionService.UpdateAnswerOptionAsync(
                req.QuestionId,
                req.AnswerOptionId,
                req.Text,
                req.Order,
                req.Points,
                req.IsCorrect,
                req.OptionNumber
            );

            if (option == null)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "Answer option not found" },
                    404,
                    cancellation: ct
                );
                return;
            }

            Response = new AnswerOptionResponse
            {
                Id = option.Id,
                OptionNumber = option.OptionNumber,
                Text = option.Text,
                Order = option.Order,
                Points = option.Points,
                IsCorrect = option.IsCorrect
            };

            await HttpContext.Response.SendAsync(Response, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendAsync(new { error = ex.Message }, 400, cancellation: ct);
        }
    }
}
