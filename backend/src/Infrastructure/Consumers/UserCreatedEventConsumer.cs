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

        // TODO: Add additional processing logic here
        // - Send welcome email
        // - Create user profile
        // - Trigger onboarding workflow
        // - Log to analytics

        return Task.CompletedTask;
    }
}
