using FastEndpoints;

namespace API.Endpoints.Health;

public class GetHealthEndpoint : EndpointWithoutRequest<HealthResponse>
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
        Description(x => x.WithTags("Health").Produces(200, typeof(HealthResponse)));
        Summary(s =>
        {
            s.Summary = "API Health Check";
            s.Description = "Returns the current health status of the API";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await HttpContext.Response.SendAsync(
            new HealthResponse { Status = "Healthy", Timestamp = DateTime.UtcNow },
            cancellation: ct
        );
    }
}

public class HealthResponse
{
    public string Status { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}
