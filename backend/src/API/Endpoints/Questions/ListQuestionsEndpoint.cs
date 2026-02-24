using API.DTOs.Questions;
using Core.Entities.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Request for paginated question list with filters.
/// </summary>
public class ListQuestionsRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchText { get; set; }
    public string? Tags { get; set; } // Comma-separated tags
    public string? QuestionType { get; set; } // e.g., "MCQ"
}

/// <summary>
/// Endpoint to list all questions with pagination and filtering.
/// </summary>
public class ListQuestionsEndpoint : Endpoint<ListQuestionsRequest, QuestionListResponse>
{
    private readonly IQuestionService _questionService;

    public ListQuestionsEndpoint(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public override void Configure()
    {
        Get("/questions");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all questions";
            s.Description =
                "Gets a paginated list of questions with optional filtering by search text, tags, and question type. Publicly accessible.";
        });
    }

    public override async Task HandleAsync(ListQuestionsRequest req, CancellationToken ct)
    {
        IEnumerable<string>? tags = null;
        if (!string.IsNullOrWhiteSpace(req.Tags))
        {
            tags = req.Tags.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
        }

        var (items, totalCount) = await _questionService.GetAllAsync(
            req.PageNumber,
            req.PageSize,
            req.SearchText,
            tags,
            req.QuestionType
        );

        Response = new QuestionListResponse
        {
            Items = items.Select(q => MapQuestion(q)).ToList(),
            TotalCount = totalCount,
            PageNumber = req.PageNumber,
            PageSize = req.PageSize
        };

        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }

    private QuestionResponse MapQuestion(Question q)
    {
        return new QuestionResponse
        {
            Id = q.Id,
            Text = q.Text,
            Description = q.Description,
            QuestionType = q.QuestionType,
            DurationSeconds = q.DurationSeconds,
            MediaMetadatas = q
                .MediaMetadatas.Select(m => new MediaMetadataDto
                {
                    Id = m.Id,
                    MediaType = m.MediaType.ToString(),
                    Url = m.Url,
                    DurationSeconds = m.DurationSeconds,
                    MimeType = m.MimeType,
                    ThumbnailUrl = m.ThumbnailUrl
                })
                .ToList(),
            Tags = q
                .QuestionTags.Select(qt => new TagResponse { Id = qt.Tag.Id, Name = qt.Tag.Name })
                .ToList(),
            CreatedBy = q.CreatedBy,
            IsVerified = q.IsVerified,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        };
    }
}
