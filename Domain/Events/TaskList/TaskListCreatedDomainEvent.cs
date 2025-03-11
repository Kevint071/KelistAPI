using Domain.Primitives;

namespace Domain.Events.TaskList
{
    public record TaskListCreatedDomainEvent(Guid Id, Guid UserId, Guid TaskListId) : DomainEvent(Id);
}
