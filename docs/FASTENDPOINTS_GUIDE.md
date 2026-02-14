# FastEndpoints Development Guide

This project uses [FastEndpoints](https://fast-endpoints.com/) for building its API. FastEndpoints is a developer-friendly alternative to Minimal APIs and MVC Controllers, following the **REPR Pattern** (Request-Endpoint-Response).

## Table of Contents

- [Core Concepts](#core-concepts)
- [Getting Started](#getting-started)
- [Endpoint Types](#endpoint-types)
- [Creating an Endpoint](#creating-an-endpoint)
- [Model Binding](#model-binding)
- [Validation](#validation)
- [Security & Authentication](#security--authentication)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Advanced Features](#advanced-features)
- [Testing](#testing)
- [Best Practices](#best-practices)
- [Important Notes](#important-notes)

---

## Core Concepts

### REPR Pattern

FastEndpoints follows the **REPR Pattern** (Request-Endpoint-Response):

- **Request**: The data coming into the endpoint (DTO)
- **Endpoint**: The logic that handles the request
- **Response**: The data going back to the client

### Key Benefits

- **Simple**: No complex controller or minimal API setup
- **Testable**: Easy integration and unit testing
- **Performant**: Minimal overhead
- **Flexible**: Multiple configuration options

---

## Getting Started

### Installation

```bash
dotnet add package FastEndpoints
```

For additional features:

```bash
# For JWT/Security support
dotnet add package FastEndpoints.Security

# For Swagger support
dotnet add package FastEndpoints.Swagger

# For Source Generator (faster startup)
dotnet add package FastEndpoints.Generator
```

### Basic Setup (Program.cs)

```csharp
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add FastEndpoints services
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "My API";
        s.Version = "v1";
    };
});

var app = builder.Build();

// Use FastEndpoints
app.UseFastEndpoints();
app.UseSwaggerGen(); // For Swagger UI

await app.RunAsync();
```

### This Project's Setup

Our project uses FastEndpoints with Scalar for API documentation:

```csharp
// See backend/src/API/Program.cs for the full configuration
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(...);

// After build
app.UseFastEndpoints();
app.UseSwaggerGen();

// Scalar API Reference
app.MapScalarApiReference(options =>
{
    options.Title = "Dhamma Session API";
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
});
```

---

## Endpoint Types

FastEndpoints provides 4 main endpoint base types:

### 1. Endpoint<TRequest>

Use when there's only a request DTO (no response DTO):

```csharp
public class CreateEndpoint : Endpoint<CreateRequest>
{
    public override void Configure()
    {
        Post("/api/items");
    }

    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        // Process request...
        await HttpContext.Response.SendAsync(new { Id = 1 }, 201);
    }
}
```

### 2. Endpoint<TRequest, TResponse>

Use when you have both request and response DTOs:

```csharp
public class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    public override void Configure()
    {
        Post("/api/users");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var user = await userService.CreateAsync(req);
        await HttpContext.Response.SendAsync(new CreateUserResponse { UserId = user.Id }, 200);
    }
}
```

### 3. EndpointWithoutRequest

Use when there's no request nor response DTO:

```csharp
public class HealthEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await HttpContext.Response.SendAsync(new { Status = "Healthy", Timestamp = DateTime.UtcNow }, 200);
    }
}
```

### 4. EndpointWithoutRequest<TResponse>

Use when there's no request DTO but there is a response DTO:

```csharp
public class GetCurrentUserEndpoint : EndpointWithoutRequest<UserResponse>
{
    public override void Configure()
    {
        Get("/api/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await userService.GetCurrentUserAsync();
        await HttpContext.Response.SendAsync(new UserResponse { Id = user.Id, Name = user.Name }, 200);
    }
}
```

### Fluent Generic Syntax

Alternatively, you can use the fluent `Ep` builder:

```csharp
// Equivalent to Endpoint<TRequest>
public class MyEndpoint : Ep.Req<MyRequest>.NoRes { }

// Equivalent to Endpoint<TRequest, TResponse>
public class MyEndpoint : Ep.Req<MyRequest>.Res<MyResponse> { }

// Equivalent to EndpointWithoutRequest
public class MyEndpoint : Ep.NoReq.NoRes { }

// Equivalent to EndpointWithoutRequest<TResponse>
public class MyEndpoint : Ep.NoReq.Res<MyResponse> { }
```

---

## Creating an Endpoint

### Step 1: Define DTOs

Create request and response models in `backend/src/API/DTOs/`:

```csharp
// Request DTO
public class CreateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// Response DTO
public class CreateUserResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public bool IsOver18 { get; set; }
}
```

### Step 2: Implement the Endpoint

Inherit from the appropriate base type:

```csharp
using FastEndpoints;

namespace API.Endpoints.Users;

public class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    private readonly IUserService _userService;

    // Dependency Injection via constructor
    public CreateUserEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Post("/api/users");
        Roles("Admin"); // Require Admin role
        Summary(s =>
        {
            s.Summary = "Create a new user";
            s.Description = "Creates a new user in the system";
        });
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var user = await _userService.CreateAsync(req);
        
        await HttpContext.Response.SendAsync(new CreateUserResponse
        {
            UserId = user.Id,
            FullName = $"{req.FirstName} {req.LastName}",
            IsOver18 = req.Age > 18
        }, 201);
    }
}
```

### Using Attributes Instead of Configure()

You can also use attributes for simpler endpoint configuration:

```csharp
[HttpPost("/api/users")]
[Authorize(Roles = "Admin")]
public class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        // Handler logic
    }
}
```

---

## Model Binding

FastEndpoints automatically binds request data from multiple sources in this order:

1. **JSON Body**
2. **Form Fields**
3. **Route Parameters**
4. **Query Parameters**
5. **User Claims** (if decorated with `[FromClaim]`)
6. **HTTP Headers** (if decorated with `[FromHeader]`)

### Binding from Route Parameters

```csharp
// Request DTO
public class GetUserRequest
{
    public Guid UserId { get; set; }
}

// Endpoint
public class GetUserEndpoint : Endpoint<GetUserRequest, UserResponse>
{
    public override void Configure()
    {
        Get("/api/users/{UserId}");
    }
}

// Request to: GET /api/users/123
// UserId will be bound to 123
```

### Binding from Query Parameters

```csharp
// Request is automatically bound from query string
public class SearchRequest
{
    public string Query { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

// Request to: GET /api/search?Query=fast&Page=2
```

### Using Attributes for Custom Binding

```csharp
public class MyRequest
{
    [FromClaim]
    public string UserId { get; set; }

    [FromHeader]
    public string TenantId { get; set; }

    [BindFrom("customer_id")]
    public string CustomerId { get; set; }
}
```

### Receiving Raw Content

```csharp
public class RawRequest : IPlainTextRequest
{
    public string Content { get; set; }
}
```

---

## Validation

FastEndpoints uses FluentValidation for automatic request validation.

### Creating a Validator

```csharp
using FastEndpoints;
using FluentValidation;

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0")
            .LessThan(150).WithMessage("Age must be less than 150");
    }
}
```

### Automatic Validation

Validators in the same namespace as the endpoint are automatically applied:

```csharp
// These are automatically hooked up if:
// 1. Validator is in the same namespace
// 2. Or validator is in the same assembly

public class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    // Validation runs automatically before HandleAsync
}
```

### Manual Validation

You can also add validation errors manually:

```csharp
public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
{
    if (await userService.EmailExistsAsync(req.Email))
    {
        AddError(r => r.Email, "Email already exists");
        ThrowIfAnyErrors();
    }
}
```

---

## Security & Authentication

### JWT Bearer Authentication

Install the security package:

```bash
dotnet add package FastEndpoints.Security
```

Setup in Program.cs:

```csharp
using FastEndpoints.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationJwtBearer(s =>
    s.SigningKey = "Your-Secret-Signing-Key-Here")
    .AddAuthorization()
    .AddFastEndpoints();

var app = builder.Build();

app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints();

await app.RunAsync();
```

### Generating JWT Tokens

```csharp
public class LoginEndpoint : Endpoint<LoginRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (await authService.ValidateCredentialsAsync(req.Username, req.Password))
        {
            var token = JwtBearer.CreateToken(o =>
            {
                o.SigningKey = "Your-Signing-Key";
                o.ExpireAt = DateTime.UtcNow.AddDays(1);
                o.User.Roles.Add("Admin", "User");
                o.User.Claims.Add(("UserId", "123"));
            });

            await HttpContext.Response.SendAsync(new TokenResponse { Token = token }, 200);
        }
        else
        {
            ThrowError("Invalid credentials");
        }
    }
}
```

### Cookie Authentication

```csharp
builder.Services.AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(60))
    .AddAuthorization()
    .AddFastEndpoints();

// Sign in from endpoint
await CookieAuth.SignInAsync(u =>
{
    u.Roles.Add("Admin");
    u.Claims.Add(("UserId", "123"));
});

// Sign out
await CookieAuth.SignOutAsync();
```

### Endpoint Authorization

Endpoints are secure by default. Use authorization methods:

```csharp
public override void Configure()
{
    Post("/api/users");
    
    // Allow anonymous access
    AllowAnonymous();
    
    // Or require authorization
    // Roles("Admin", "Manager");
    // Policies("CanWrite");
    // Claims("UserId");
    // Permissions("CanCreateUser");
}
```

### Pre-Built Security Policies

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagersOnly", x => 
        x.RequireRole("Manager").RequireClaim("ManagerID"));
});

// Endpoint
public override void Configure()
{
    Put("/api/users");
    Policies("ManagersOnly");
}
```

### Source Generated Permissions

Generate permission constants automatically:

```csharp
public class CreateArticleEndpoint : Endpoint<CreateArticleRequest>
{
    public override void Configure()
    {
        Post("/api/articles");
        AccessControl("Article_Create", behavior: Apply.ToThisEndpoint);
        Permissions(Allow.Article_Create); // Requires FastEndpoints.Generator
    }
}
```

---

## Configuration

### JSON Serializer Options

```csharp
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});
```

### Global Route Prefix

```csharp
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
// GET /users becomes GET /api/users
```

### Endpoint Configuration Groups

Group endpoints with shared configuration:

```csharp
// Define a group
public class AdminGroup : Group
{
    public AdminGroup()
    {
        Configure("admin", ep =>
        {
            ep.Description(x => x.Produces(401));
            ep.Tags("Administration");
        });
    }
}

// Sub-group
public class UsersAdmin : SubGroup<AdminGroup>
{
    public UsersAdmin()
    {
        Configure("users", ep => { });
    }
}

// Use in endpoint
public class CreateUserEndpoint : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/create");
        Group<UsersAdmin>(); // Results in /admin/users/create
    }
}
```

### Filtering Endpoint Registration

```csharp
app.UseFastEndpoints(c =>
{
    c.Endpoints.Filter = ep =>
    {
        // Don't register deprecated endpoints
        if (ep.EndpointTags?.Contains("Deprecated") == true)
            return false;
        return true;
    };
});
```

### Custom Error Responses (RFC 7807)

```csharp
app.UseFastEndpoints(c =>
{
    c.Errors.UseProblemDetails();
});
```

---

## API Documentation

### Swagger/Scalar Setup

This project uses Scalar for API documentation:

```csharp
// Program.cs
builder.Services.AddSwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Dhamma Session API";
        s.Version = "v1";
    };
});

app.UseFastEndpoints();
app.UseSwaggerGen();

app.MapScalarApiReference(options =>
{
    options.Title = "Dhamma Session API";
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
    options.ShowSidebar = true;
    options.DarkMode = true;
});
```

### Adding Documentation to Endpoints

```csharp
public override void Configure()
{
    Get("/health");
    AllowAnonymous();
    
    Description(x => x
        .WithTags("Health")
        .Produces<HealthResponse>(200)
        .Produces(401));
    
    Summary(s =>
    {
        s.Summary = "API Health Check";
        s.Description = "Returns the current health status of the API";
        s.Response<HealthResponse>(200, "Returns health status");
    });
}
```

### Accessing Documentation

- **Swagger UI**: `http://localhost:5001/swagger`
- **Scalar**: `http://localhost:5001/scalar/v1`

---

## Advanced Features

### Pre/Post Processors

```csharp
// Pre-processor
public class MyPreProcessor : IPreProcessor<MyRequest>
{
    public Task PreProcessAsync(MyRequest req, HttpContext ctx, CancellationToken ct)
    {
        // Run before handler
        return Task.CompletedTask;
    }
}

// Post-processor
public class MyPostProcessor : IPostProcessor<MyRequest, MyResponse>
{
    public Task PostProcessAsync(MyRequest req, MyResponse res, HttpContext ctx, CancellationToken ct)
    {
        // Run after handler
        return Task.CompletedTask;
    }
}

// Use in endpoint
public override void Configure()
{
    Post("/api/test");
    PreProcessor<MyPreProcessor>();
    PostProcessor<MyPostProcessor>();
}
```

### Response Sending Methods

⚠️ **IMPORTANT**: Always use `HttpContext.Response.SendAsync()` instead of `SendAsync()` to avoid compilation errors:

```csharp
// ✅ CORRECT - Always use this
await HttpContext.Response.SendAsync(response, 200);
await HttpContext.Response.SendAsync(response, 201);

// ❌ WRONG - Do NOT use this (will cause CS0103 errors)
await SendAsync(response);
await SendAsync(response, 201);
```

The full signature is:
```csharp
await HttpContext.Response.SendAsync<T>(T response, int statusCode);
```

### Send Specific Status Codes

```csharp
// Send 200 OK
await HttpContext.Response.SendAsync(response, 200);

// Send 201 Created
await HttpContext.Response.SendAsync(response, 201);

// Send 204 No Content - send empty object
await HttpContext.Response.SendAsync(new { }, 204);

// Send 400 Bad Request - use ThrowError
ThrowError("Error message");

// Send 404 Not Found - return early with empty response
return;
```

### Using TypedResults (Union Types)

```csharp
public class MyEndpoint : Endpoint<MyRequest, Results<Ok<MyResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/items/{Id}");
    }

    public override async Task<Results<Ok<MyResponse>, NotFound>> ExecuteAsync(
        MyRequest req, CancellationToken ct)
    {
        var item = await itemService.GetAsync(req.Id);
        
        if (item == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(new MyResponse { ... });
    }
}
```

### Source Generators

For faster startup in serverless environments:

```csharp
// Install
dotnet add package FastEndpoints.Generator

// Program.cs
builder.Services.AddFastEndpoints(o =>
    o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);
```

And add reflection cache:

```csharp
app.UseFastEndpoints(c =>
    c.Binding.ReflectionCache.AddFromMyApp());
```

---

## Testing

### Unit Testing

```csharp
public class CreateUserEndpointTest
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsCreatedUser()
    {
        // Arrange
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid() });

        var endpoint = new CreateUserEndpoint(mockService.Object);
        
        // Set up test context
        endpoint.Configure();
        
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Age = 25
        };

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.CreateAsync(It.IsAny<CreateUserRequest>()), Times.Once);
    }
}
```

### Integration Testing

```csharp
public class ApiTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(defaultScheme: "Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
        });
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test User") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

---

## Best Practices

### 1. Project Structure

Organize endpoints by feature:

```
backend/src/API/
├── DTOs/
│   ├── Users/
│   │   ├── CreateUserRequest.cs
│   │   └── CreateUserResponse.cs
│   └── Auth/
├── Endpoints/
│   ├── Users/
│   │   ├── CreateUserEndpoint.cs
│   │   └── GetUserEndpoint.cs
│   └── Auth/
│       └── LoginEndpoint.cs
└── Validators/
    └── CreateUserRequestValidator.cs
```

### 2. Dependency Injection

Use constructor injection for services:

```csharp
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    private readonly IMyService _service;

    public MyEndpoint(IMyService service)
    {
        _service = service;
    }
}
```

### 3. Always Use HttpContext.Response.SendAsync

⚠️ **CRITICAL**: Always use `HttpContext.Response.SendAsync()` for sending responses:

```csharp
// ✅ CORRECT
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var result = await _service.ProcessAsync(req, ct);
    await HttpContext.Response.SendAsync(result, 200);
}

// ❌ WRONG - Will cause compilation errors
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var result = await _service.ProcessAsync(req, ct);
    await SendAsync(result);
}
```

### 4. Use Cancellation Tokens

Always pass cancellation tokens to downstream services:

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    await _service.LongRunningOperationAsync(ct);
}
```

### 5. Security

- Always use `[AllowAnonymous()]` only when necessary
- Use role/policy-based authorization
- Validate all input with FluentValidation
- Never expose sensitive data in responses

### 6. Error Handling

Use `ThrowError()` for validation errors:

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    if (!ModelState.IsValid)
        AddError("Invalid model state");
    
    if (someBusinessRule)
        AddError(r => r.Property, "Business rule violation");
    
    ThrowIfAnyErrors();
}
```

### 7. Documentation

Always add Summary and Description to endpoints:

```csharp
public override void Configure()
{
    Post("/api/users");
    Summary(s =>
    {
        s.Summary = "Create a new user";
        s.Description = "Creates a new user and returns the user details";
    });
}
```

### 8. Versioning

For API versioning, use `Asp.Versioning.Http`:

```csharp
public override void Configure()
{
    Get("/api/v{version:apiVersion}/users");
    Version(1, 2);
}
```

---

## Important Notes

### ⚠️ Critical: Always Use HttpContext.Response.SendAsync

**This is the most important rule in this project:**

In this project's setup with .NET 10 and FastEndpoints 7.2.0, you MUST always use `HttpContext.Response.SendAsync()` instead of the shorter `SendAsync()` method. The shorter version causes CS0103 compilation errors ("The name 'SendAsync' does not exist in the current context").

**Correct Usage:**
```csharp
// With status code
await HttpContext.Response.SendAsync(response, 200);

// Without status code (not recommended)
await HttpContext.Response.SendAsync(response);
```

**Wrong Usage (will cause errors):**
```csharp
// ❌ DO NOT USE - Causes CS0103 error
await SendAsync(response);

// ❌ DO NOT USE - Causes CS0103 error  
await SendAsync(response, 201);
```

This applies to all endpoint types:
- `Endpoint<TRequest, TResponse>`
- `EndpointWithoutRequest<TResponse>`
- `Endpoint<TRequest>`
- `EndpointWithoutRequest`

---

## Accessing the API Documentation

When the API is running, you can view the documentation at:

- **Swagger UI**: `http://localhost:5001/swagger`
- **Scalar API Reference**: `http://localhost:5001/scalar/v1`

---

## Common Issues & Solutions

### Endpoints Not Discovered

If endpoints aren't being registered, check:

1. Endpoints are in a non-excluded assembly name
2. Endpoints inherit from `Endpoint<TRequest, TResponse>` or similar
3. Endpoints are in the same assembly as Program.cs

### Validation Not Running

1. Ensure FluentValidation package is installed
2. Validator must be in the same namespace/assembly
3. Or register validators explicitly in DI

### SendAsync Does Not Exist (CS0103)

**This is the most common error.** The solution is simple:

```csharp
// ❌ Wrong - causes CS0103
await SendAsync(response);

// ✅ Correct - always use HttpContext.Response
await HttpContext.Response.SendAsync(response, 200);
```

### Route Conflicts

If you have route conflicts, check:
- Multiple endpoints with same route and method
- Conflicting route prefixes

---

## Additional Resources

- [Official FastEndpoints Documentation](https://fast-endpoints.com/docs/)
- [FastEndpoints GitHub](https://github.com/FastEndpoints/Documentation)
- [FastEndpoints Discord](https://discord.gg/fastendpoints)
