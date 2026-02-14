using API.Hubs;
using FastEndpoints;
using FastEndpoints.Swagger;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Dhamma Session API";
        s.Version = "v1";
    };
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
app.UseFastEndpoints();
app.UseSwaggerGen();

app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    options.Title = "Dhamma Session API";
    options.Theme = ScalarTheme.Default;
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
    options.ShowSidebar = true;
    options.DarkMode = true;
});

app.UseHttpsRedirection();

app.UseCors("SignalRCors");

// Map SignalR hub
app.MapHub<SessionHub>("/hub/session");

await app.RunAsync();
