using Core.Events;
using Core.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consumers;

public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserCreatedEventConsumer> _logger;

    public UserCreatedEventConsumer(
        IUserProfileService userProfileService,
        ILogger<UserCreatedEventConsumer> logger
    )
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var userEvent = context.Message;

        _logger.LogInformation(
            "User created event received: UserId={UserId}, Email={Email}, FirstName={FirstName}, LastName={LastName}",
            userEvent.UserId,
            userEvent.Email,
            userEvent.FirstName,
            userEvent.LastName
        );

        // Create and save UserProfile from the event
        var userProfile = await _userProfileService.CreateFromEventAsync(userEvent);

        _logger.LogInformation(
            "UserProfile saved: UserId={UserId}, FirstName={FirstName}, LastName={LastName}",
            userProfile.UserId,
            userProfile.FirstName,
            userProfile.LastName
        );

        // TODO: Additional processing:
        // - Send welcome email
        // - Trigger onboarding workflow
        // - Log to analytics
    }
}
