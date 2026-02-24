using API.DTOs.Questions;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.Questions;

/// <summary>
/// Request for searching tags (typeahead).
/// </summary>
public class SearchTagsRequest
{
    /// <summary>
    /// Search text for tag name
    /// </summary>
    public string? Search { get; set; }
}

/// <summary>
/// Endpoint to search tags (typeahead behavior).
/// </summary>
public class SearchTagsEndpoint : Endpoint<SearchTagsRequest, List<TagResponse>>
{
    private readonly ITagService _tagService;

    public SearchTagsEndpoint(ITagService tagService)
    {
        _tagService = tagService;
    }

    public override void Configure()
    {
        Get("/tags");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Search tags";
            s.Description = "Searches tags by name (typeahead). Returns top 10 results ordered by popularity.";
        });
    }

    public override async Task HandleAsync(SearchTagsRequest req, CancellationToken ct)
    {
        var tags = await _tagService.SearchAsync(req.Search ?? string.Empty);

        Response = tags
            .Select(t => new TagResponse { Id = t.Id, Name = t.Name })
            .ToList();

        await HttpContext.Response.SendAsync(Response, cancellation: ct);
    }
}