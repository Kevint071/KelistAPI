using Domain.Primitives;
using Domain.Users;

namespace Domain.Events.User
{
    public record UserCreatedEvent(UserId UserId, string FullName, string Email) : DomainEvent(Guid.NewGuid());
}
