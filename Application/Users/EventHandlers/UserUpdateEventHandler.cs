using Domain.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers
{
    public class UserUpdateEventHandler : INotificationHandler<UserUpdateEvent>
    {
        private readonly ILogger<UserUpdateEventHandler> _logger;

        public UserUpdateEventHandler(ILogger<UserUpdateEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(UserUpdateEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("The user with Id \"{UserId}\" updated their information: Full Name \"{FullName}\" and Email \"{Email}\".", notification.UserId, notification.FullName, notification.Email);
            return Task.CompletedTask;
        }
    }
}
