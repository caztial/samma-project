using Core.Entities.UserProfiles;
using Core.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consumers;

public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventConsumer> _logger;

    public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var userEvent = context.Message;

        _logger.LogInformation(
            "User created event received: UserId={UserId}, Email={Email}, FirstName={FirstName}, LastName={LastName}",
            userEvent.UserId,
            userEvent.Email,
            userEvent.FirstName,
            userEvent.LastName
        );

        // Create UserProfile from the event
        // Note: The actual database save would be handled by a unit of work or repository
        // This consumer publishes the creation request or delegates to a handler
        var userProfile = UserProfile.CreateFromEvent(userEvent);

        _logger.LogInformation(
            "UserProfile created: UserId={UserId}, FirstName={FirstName}, LastName={LastName}",
            userProfile.UserId,
            userProfile.FirstName,
            userProfile.LastName
        );

        // TODO: Additional processing:
        // - Save userProfile to database
        // - Send welcome email
        // - Trigger onboarding workflow
        // - Log to analytics

        return Task.CompletedTask;
    }
}
