using Domain.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("The user \"{FullName}\" with Email \"{Email}\" and Id \"{UserId}\" was created.", notification.FullName, notification.Email, notification.UserId.Value);
            return Task.CompletedTask;
        }
    }
}
