# System Patterns

## Domain-Driven Design

### Core Aggregates
1. **User** - Participant/Admin/Moderator accounts
2. **Question** - Question bank items with answer options
3. **Session** - Dhamma session lifecycle
4. **Answer** - Participant answer submissions
5. **AuditLog** - Activity tracking

### Domain Events
```csharp
public record SessionStartedEvent(Guid SessionId, string Code);
public record QuestionPushedEvent(Guid SessionId, Guid QuestionId);
public record AnswerSubmittedEvent(Guid SessionId, Guid UserId, Guid QuestionId);
public record SessionEndedEvent(Guid SessionId);
```

## Clean Architecture Layers

### API Layer
- Controllers for HTTP endpoints
- SignalR Hubs for real-time
- DTOs for request/response
- Input validation

### Core Layer (Domain + Application)
- Entities
- Value objects
- Domain events
- Repository interfaces
- Commands/Queries (CQRS)
- Event handlers
- Business logic orchestration

### Infrastructure Layer
- EF Core DbContext
- Repository implementations
- SignalR configuration
- External services

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
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### Specific Repositories
```csharp
public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> SearchAsync(string query, string[] tags);
}

public interface ISessionRepository : IRepository<Session>
{
    Task<Session> GetByCodeAsync(string code);
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

### JWT Flow
```
1. Login → Validate credentials
2. Generate JWT with claims (userId, role)
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
