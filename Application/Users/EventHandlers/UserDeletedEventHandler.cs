using Domain.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers
{
    public class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventHandler> _logger;

        public UserDeletedEventHandler(ILogger<UserDeletedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("The user \"{FullName}\" with Id \"{UserId}\" was deleted.", notification.FullName, notification.UserId);
            return Task.CompletedTask;
        }
    }
}
