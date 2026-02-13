using API.Hubs;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = "http://localhost:5001" }
            };
            return Task.CompletedTask;
        }
    );
});

// Add SignalR
builder.Services.AddSignalR();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "SignalRCors",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Dhamma Session API";
    options.Theme = ScalarTheme.Mars;
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
    options.ShowSidebar = true;
    options.DarkMode = true;
});

app.UseHttpsRedirection();

app.UseCors("SignalRCors");

// Map SignalR hub
app.MapHub<SessionHub>("/hub/session");

// Minimal API endpoints
app.MapGet("/", () => Results.Ok(new { message = "Dhamma Session API is running!" }))
    .WithName("GetRoot")
    .WithOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
    .WithName("GetHealth")
    .WithOpenApi();

app.Run();
