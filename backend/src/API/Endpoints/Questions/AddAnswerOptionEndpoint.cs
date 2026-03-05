using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Endpoint to add an answer option to an MCQ question.
/// </summary>
public class AddAnswerOptionEndpoint : Endpoint<AddAnswerOptionRequest, AnswerOptionResponse>
{
    private readonly IMcqQuestionService _mcqQuestionService;

    public AddAnswerOptionEndpoint(IMcqQuestionService mcqQuestionService)
    {
        _mcqQuestionService = mcqQuestionService;
    }

    public override void Configure()
    {
        Post("/questions/mcq/{QuestionId}/answers");
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Add an answer option to an MCQ question";
            s.Description =
                "Adds an answer option to an MCQ question. Requires Admin or Moderator role.";
        });
    }

    public override async Task HandleAsync(AddAnswerOptionRequest req, CancellationToken ct)
    {
        try
        {
            var option = await _mcqQuestionService.AddAnswerOptionAsync(
                req.QuestionId,
                req.Text,
                req.Order,
                req.Points,
                req.IsCorrect,
                req.OptionNumber
            );

            if (option == null)
            {
                await HttpContext.Response.SendAsync(
                    new { error = "MCQ Question not found" },
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
