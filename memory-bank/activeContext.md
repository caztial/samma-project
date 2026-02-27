# Active Context

## Current Phase: Async Answer Submission Implementation ✅

Recent work completed on answer submission refactoring:

### Problem Statement
The `SubmitMCQAnswerEndpoint` was processing answers synchronously, which could cause issues during high-frequency answer submission periods (many participants answering simultaneously).

### Solution: Queue-Based Async Processing
Implemented async command pattern using MassTransit for queue-based processing:

1. **Endpoint publishes command** → Returns 202 Accepted immediately
2. **MassTransit queue** → Commands processed in order
3. **Consumer processes** → Calls SessionService, handles errors
4. **SignalR notification** → Result sent to specific user

### Route Structure Change
- **Old**: `POST /sessions/{Id}/answers`
  - Body: `{ QuestionId, AttemptNumber, SelectedOptionId }`
- **New**: `POST /sessions/{Id}/questions/{QuestionId}/attempts/{AttemptNumber}/answers`
  - Body: `{ SelectedOptionId }`
  - Response: `202 Accepted` with `{ CommandId, Status, Message, SessionId, QuestionId, AttemptNumber }`

### New Events/Commands Added

#### SubmitAnswerCommand
```csharp
public record SubmitAnswerCommand
{
    public Guid CommandId { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public Guid QuestionId { get; init; }
    public int AttemptNumber { get; init; }
    public Guid SelectedOptionId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
```

#### AnswerSubmissionResultEvent
```csharp
public record AnswerSubmissionResultEvent
{
    public Guid CommandId { get; init; }
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
    public Guid SessionId { get; init; }
    public Guid QuestionId { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? AnswerId { get; init; }
    public int AttemptNumber { get; init; }
    public Guid SelectedOptionId { get; init; }
    public bool IsCorrect { get; init; }
    public double Score { get; init; }
}
```

### Idempotency Implementation
The `SubmitAnswerCommandConsumer` includes in-memory idempotency tracking:
- Tracks processed `CommandId` values in a `HashSet<Guid>`
- Skips duplicate commands with warning log
- Auto-cleanup when exceeding 10,000 entries
- **Note**: For production, replace with distributed cache (Redis)

### SignalR Client Integration
Clients should:
1. POST to endpoint → receive `202 Accepted` with `CommandId`
2. Listen for `AnswerResult` event on SignalR
3. Match `CommandId` to correlate response
4. Handle both success (`Success: true`) and error (`Success: false`, `ErrorMessage`)

### Files Modified
| File | Change |
|------|--------|
| Core/Events/SessionEvents.cs | Added SubmitAnswerCommand, AnswerSubmissionResultEvent |
| API/DTOs/Sessions/SessionDtos.cs | Added SubmitMCQAnswerRequestBody, SubmitAnswerAcceptedResponse |
| API/Endpoints/Sessions/AnswerAndScoringEndpoints.cs | Refactored endpoint with route params and async publishing |
| API/Consumers/SessionEventConsumers.cs | Added SubmitAnswerCommandConsumer with idempotency |
| API/Program.cs | Registered SubmitAnswerCommandConsumer in MassTransit |

## Key Design Decisions

1. **Async Everything**: All validation happens in the consumer, errors sent via SignalR
2. **Idempotency**: CommandId prevents duplicate processing (essential for retries)
3. **User-Specific Notification**: Results sent to specific user via `Clients.User(userId)`
4. **202 Accepted Pattern**: Client gets immediate response, processes result async
5. **Route Parameters**: sessionId, questionId, attemptNumber from route, only SelectedOptionId in body

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Next Steps
1. Replace in-memory idempotency with Redis distributed cache
2. Add frontend SignalR client integration
3. Consider rate limiting per user/session
4. Add monitoring/metrics for queue processing times