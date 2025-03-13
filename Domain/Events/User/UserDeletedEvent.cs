using Domain.Primitives;
using Domain.Users;

namespace Domain.Events.User
{
    public record UserDeletedEvent(Guid UserId, string FullName) : DomainEvent(Guid.NewGuid());
}
