using API.Hubs;
using API.Security;
using Core.Entities;
using Core.Entities.UserProfiles;
using Core.Repositories;
using Core.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Infrastructure.Consumers;
using Infrastructure.Data;
using Infrastructure.PostgreSQL.Repositories;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddEnvironmentVariables();

// ============================================
// JWT OPTIONS
// ============================================
var jwtOptions =
    builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration not found in appsettings.json");

builder.Services.AddSingleton(jwtOptions);

// ============================================
// DATABASE & IDENTITY
// ============================================
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register Data Protection
builder.Services.AddDataProtection();

// Register Encryption Service
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// Register DbContext with encryption service
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ============================================
// JWT AUTHENTICATION (FastEndpoints.Security)
// ============================================

builder.Services.AddAuthenticationJwtBearer(s =>
{
    s.SigningKey = jwtOptions.SigningKey;
});

// Add authorization services
builder.Services.AddAuthorization();

// Configure default schemes to JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
builder.Services.AddFastEndpoints();

// Register the AdminOwnerAuthorizationHandler
builder.Services.AddScoped<IAuthorizationHandler, AdminOwnerAuthorizationHandler>();

// Register resource owner authorization implementations as keyed services
// Key = AggregatedRootName (e.g., "UserProfile")
builder.Services.AddKeyedScoped<IResourceOwnerAuthorization, ProfileResourceOwnerAuthorization>(
    nameof(UserProfile)
);

// ============================================
// MASS TRANSIT
// ============================================
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedEventConsumer>();

    x.UsingInMemory(
        (context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        }
    );
});

// ============================================
// REPOSITORIES & SERVICES
// ============================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// ============================================
// FAST ENDPOINTS
// ============================================
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Dhamma Session API";
        s.Version = "v1";
    };
});

// ============================================
// CORS
// ============================================
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

// ============================================
// SIGNALR
// ============================================
builder.Services.AddSignalR();

var app = builder.Build();

// ============================================
// ENSURE DATABASE CREATED & SEED DATA
// ============================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var encryptionService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();

    // Create database and Identity tables (or apply migrations)
    await context.Database.EnsureCreatedAsync();

    // Now seed data with decrypted password
    await SeedData.SeedAsync(userManager, roleManager, encryptionService, builder.Configuration);
}

// ============================================
// CONFIGURE PIPELINE
// ============================================
app.UseCors("SignalRCors");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();

// Scalar API Reference
app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    options.Title = "Dhamma Session API";
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
    options.ShowSidebar = true;
    options.DarkMode = true;
});

// Map SignalR hub
app.MapHub<SessionHub>("/hub/session");

await app.RunAsync();
