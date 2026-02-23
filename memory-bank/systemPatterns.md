# System Patterns

## Domain-Driven Design

### Core Aggregates
1. **User** - Participant/Admin/Moderator accounts (ASP.NET Identity)
2. **UserProfile** - Extended user information with PII encryption
3. **Question** - Question bank items with answer options
4. **Session** - Dhamma session lifecycle
5. **Answer** - Participant answer submissions
6. **AuditLog** - Activity tracking

### Domain Events
```csharp
public record UserCreatedEvent(string UserId, string Email, string FirstName, string LastName);
public record SessionStartedEvent(Guid SessionId, string Code);
public record QuestionPushedEvent(Guid SessionId, Guid QuestionId);
public record AnswerSubmittedEvent(Guid SessionId, Guid UserId, Guid QuestionId);
public record SessionEndedEvent(Guid SessionId);
```

### UserProfile Aggregate Root
The UserProfile is an aggregate root that encapsulates user personal information with PII encryption:

```csharp
public class UserProfile : IAggregatedRoot
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    
    [Encrypt]
    public string FirstName { get; private set; }
    
    [Encrypt]
    public string LastName { get; private set; }
    
    public Gender Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    
    // Value Objects
    public Contact? Contact { get; private set; }
    public Biometrics? Biometrics { get; private set; }
    public List<Address> Addresses { get; private set; }
    public List<EmergencyContact> EmergencyContacts { get; private set; }
    public List<Identification> Identifications { get; private set; }
    public List<Consent> Consents { get; private set; }
}
```

## Clean Architecture Layers

### API Layer
- FastEndpoints for HTTP endpoints
- SignalR Hubs for real-time
- DTOs for request/response
- Input validation

### Core Layer (Domain + Application)
- Entities (UserProfile, ApplicationUser)
- Value objects (Contact, Address, Consent, etc.)
- Domain events (UserCreatedEvent)
- Repository interfaces
- Service interfaces (IAuthService, IEncryptionService, IUserProfileService)
- Event handlers
- Business logic orchestration

### Infrastructure Layer
- EF Core DbContext
- Repository implementations
- Service implementations (AuthService, EncryptionService)
- SignalR configuration
- External services

## PII Encryption Pattern

### Encryption Service Interface (Core Layer)
```csharp
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
```

### Encryption Service Implementation (Infrastructure Layer)
```csharp
public class EncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public EncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("UserProfile.PII.v1");
    }

    public string Encrypt(string plainText)
    {
        return _protector.Protect(plainText);
    }

    public string Decrypt(string cipherText)
    {
        return _protector.Unprotect(cipherText);
    }
}
```

### EF Core ValueConverter
```csharp
public class EncryptionConverter : ValueConverter<string, string>
{
    public EncryptionConverter(IEncryptionService encryptionService)
        : base(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v)
        )
    {
    }
}
```

### Encrypt Attribute
```csharp
[AttributeUsage(AttributeTargets.Property)]
public class EncryptAttribute : Attribute { }
```

### DbContext Configuration
```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .Properties<string>()
        .Where(p => p.GetCustomAttribute<EncryptAttribute>() != null)
        .HaveConversion<EncryptionConverter>();
}
```

## Real-time Communication Pattern

### SignalR Hub Structure
```
SessionHub
├── JoinSession(code)          → Participant joins
├── PushQuestion(qid)          → Admin pushes question
├── SubmitAnswer(data)         → Participant answers
├── JoinAsPresenter()          → Projector view
└── Broadcast methods
    ├── QuestionPushed()
    ├── AnswerResult()
    └── ShowResults()
```

### Message Flow
```
Admin → Hub → Group Broadcast → Participants
Participant → Hub → Group → Admin/Presenter
```

## Repository Pattern

### Generic Repository
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
```

### Specific Repositories
```csharp
public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
}

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<bool> ExistsAsync(string userId);
}
```

## CQRS Pattern

### Commands (Write)
```csharp
public record CreateSessionCommand(string Name) : IRequest<SessionDto>;
public record PushQuestionCommand(Guid SessionId, Guid QuestionId) : IRequest;
public record SubmitAnswerCommand(Guid SessionId, Guid QuestionId, int Option) : IRequest<AnswerResult>;
```

### Queries (Read)
```csharp
public record GetSessionQuery(Guid Id) : IRequest<SessionDto>;
public record SearchQuestionsQuery(string SearchText) : IRequest<IEnumerable<QuestionDto>>;
public record GetSessionResultsQuery(Guid SessionId) : IRequest<SessionResultsDto>;
```

## Authentication Pattern

### IAuthService Interface (Core Layer)
```csharp
public interface IAuthService
{
    Task<(ApplicationUser? User, string? Token)> LoginAsync(string email, string password);
    Task<(ApplicationUser? User, string? Token)> RegisterAsync(string email, string password, string firstName, string lastName);
}
```

### AuthService Implementation (Infrastructure Layer)
```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly JwtOptions _jwtOptions;

    public async Task<(ApplicationUser? User, string? Token)> LoginAsync(string email, string password)
    {
        // Validate credentials and generate JWT token
    }

    public async Task<(ApplicationUser? User, string? Token)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        // Create user, publish event, generate JWT token
    }
}
```

### JWT Flow
```
1. Login → IAuthService.LoginAsync validates credentials
2. Generate JWT with claims (userId, email, role)
3. Return token to client
4. Client sends token in Authorization header
5. Server validates token on each request
```

### Role-Based Access
```csharp
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Moderator,Admin")]
[Authorize] // Any authenticated user
```

### JWT Configuration (appsettings.json)
```json
{
  "Jwt": {
    "SigningKey": "YourSuperSecretKeyForJWTAuthentication2024!",
    "ExpireDays": 1
  }
}
```

### Program.cs Setup
```csharp
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() 
    ?? throw new InvalidOperationException("JWT configuration not found");

builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthenticationJwtBearer(s =>
    s.SigningKey = jwtOptions.SigningKey
)
.AddAuthorization()
.AddFastEndpoints();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
```

### FastEndpoints Response Pattern
⚠️ **IMPORTANT**: Always use `HttpContext.Response.SendAsync()` in FastEndpoints endpoints:

```csharp
// ✅ CORRECT
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var result = await _service.ProcessAsync(req, ct);
    await HttpContext.Response.SendAsync(result);
}

// ❌ WRONG - Causes CS0103 compilation errors
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var result = await _service.ProcessAsync(req, ct);
    await SendAsync(result);  // DO NOT USE
}
```

This applies to all endpoint types:
- `Endpoint<TRequest, TResponse>`
- `EndpointWithoutRequest<TResponse>`
- `Endpoint<TRequest>`
- `EndpointWithoutRequest`

### FastEndpoints Response Type Documentation
```csharp
public override void Configure()
{
    Post("/api/auth/login");
    AllowAnonymous();
    Description(x => x
        .Produces<LoginResponse>(200)
        .Produces(401)
    );
}
```

## Session State Pattern

### Session Lifecycle
```
Draft → Active → Paused → Ended
  │       │        │        │
  │       │        │        └─ No more interactions
  │       │        └─ Can resume to Active
  │       └─ Can push questions, participants can answer
  └─ Setup phase, not yet joinable
```

### State Transitions
```csharp
public async Task ActivateAsync(Guid sessionId)
{
    var session = await _repository.GetByIdAsync(sessionId);
    session.Activate(); // Domain logic
    await _repository.UpdateAsync(session);
}
```

## Multiple Attempts Scoring Pattern

### Attempt Calculation
```
Attempt 1: 100% of base points
Attempt 2: 50% of base points
Attempt 3: 25% of base points
Attempt 4+: 0 points
```

### Cooldown Period
- Time delay between attempts (configurable)
- Frontend countdown
- Server validates timing

## Audit Logging Pattern

### Automatic Auditing
```csharp
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<...>
{
    public async Task<TResponse> Handle(...)
    {
        var response = await next();
        await _auditService.LogAsync(request, response);
        return response;
    }
}
```

### Logged Actions
- Session join/leave
- Question push
- Answer submission
- Score calculation

## Value Object Patterns

### ValueObject Base Class (DDD Pattern)
Following Microsoft's DDD guidelines, all value objects inherit from an abstract base class:

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
```

### Value Object Implementation Pattern
Value objects implement `GetEqualityComponents()` to define equality:

```csharp
public sealed class Address : ValueObject
{
    public Guid Id { get; set; }  // For EF Core (1:N relationships)
    public string Line1 { get; set; } = string.Empty;
    public string Suburb { get; set; } = string.Empty;
    // ...

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id excluded from equality - value objects compare by value, not identity
        yield return Line1;
        yield return Suburb;
        // ...
    }
}
```

### 1:N Value Objects with EF Core
For value objects stored in separate tables (1:N relationship):
- Keep `Id` property for EF Core primary key
- Keep `UserProfileId` foreign key for navigation
- **Exclude `Id` from `GetEqualityComponents()`** - ensures value semantics

### Identification Value Object (Refactored)
```csharp
public sealed class Identification : ValueObject
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public string Type { get; set; } = string.Empty;  // e.g., "Passport", "SSN"
    
    [Encrypt]
    public string Value { get; set; } = string.Empty;  // e.g., "N60019023"
    
    public Guid UserProfileId { get; set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id excluded from equality
        yield return Type;
        yield return Value;
    }
}
```

### Contact (PII - Encrypted, 1:1)
```csharp
public sealed class Contact : ValueObject
{
    [Encrypt]
    public string ContactNumber { get; private set; } = string.Empty;
    
    [Encrypt]
    public string Email { get; private set; } = string.Empty;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ContactNumber;
        yield return Email;
    }
}
```

### Consent (1:N)
```csharp
public sealed class Consent : ValueObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TermId { get; set; } = string.Empty;
    public string TermLink { get; set; } = string.Empty;
    public string TermsVersion { get; set; } = string.Empty;
    public DateTime AcceptedAt { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public Guid UserProfileId { get; set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Id excluded from equality
        yield return TermId;
        yield return TermLink;
        yield return TermsVersion;
        yield return AcceptedAt;
        yield return IpAddress;
    }
}
```

## Seed Data Pattern

### Reading Encrypted Credentials
```csharp
public static async Task SeedAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IEncryptionService encryptionService,
    IConfiguration configuration
)
{
    var adminConfig = configuration.GetSection("AdminUser");
    var encryptedPassword = adminConfig["EncryptedPassword"];
    
    // Decrypt the password
    var decryptedPassword = encryptionService.Decrypt(encryptedPassword);
    
    // Create user with decrypted password
    var result = await userManager.CreateAsync(user, decryptedPassword);
}
```

### Configuration Structure
```json
{
  "AdminUser": {
    "Email": "admin@dhamma.org",
    "EncryptedPassword": "CfDJ8...",
    "FirstName": "Admin",
    "LastName": "User"
  }
}
```

## FastEndpoints Claim Binding Pattern

### FromClaim Attribute
Use `[FromClaim]` to automatically bind properties from JWT claims:

```csharp
public class UpdateProfileRequest
{
    /// <summary>
    /// User ID from JWT claims. Used when profile ID is not provided in route.
    /// </summary>
    [FromClaim("sub")]
    public string? UserId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // ...
}
```

### Optional Route Parameters with Claim Fallback
When route ID is optional, derive ProfileId from UserId in claims:

```csharp
public override async Task HandleAsync(UpdateProfileRequest request, CancellationToken ct)
{
    Guid profileId;

    // Route ID takes precedence over claims
    if (request.Id.HasValue)
    {
        profileId = request.Id.Value;
    }
    else
    {
        // Derive from UserId in JWT claims
        var userId = request.UserId;
        
        if (string.IsNullOrEmpty(userId))
        {
            await HttpContext.Response.SendAsync(
                new { error = "Unable to identify user" },
                401,
                cancellation: ct
            );
            return;
        }

        var profile = await _userProfileService.GetByUserIdAsync(userId);
        profileId = profile.Id;
    }

    // ... rest of handler
}
```

## FastEndpoints Validation Pattern

### Validator Base Class
Validators inherit from `Validator<TRequest>`:

```csharp
using FastEndpoints;
using FluentValidation;

public class UpdateProfileRequestValidator : Validator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(3)
            .WithMessage("First name must be at least 3 characters");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required")
            .Must(BeAValidGender)
            .WithMessage("Gender must be one of: Male, Female, Other, PreferNotToSay");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(BeAValidDate)
            .WithMessage("Date of birth must be a valid date");

        // Use built-in validator for email
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
    }

    private bool BeAValidGender(string? gender)
    {
        if (string.IsNullOrEmpty(gender))
            return false;

        var validGenders = new[] { "Male", "Female", "Other", "PreferNotToSay" };
        return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeAValidDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return false;

        return DateOnly.TryParse(dateStr, out _);
    }
}
```

### Flattened DTOs
For simpler JSON payloads, flatten nested objects:

```csharp
// Before: Nested Contact object
public class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public ContactDto? Contact { get; set; }
}

// After: Flattened fields
public class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
}
```
