using Domain.Primitives;

namespace Domain.Events.TaskItem
{
    public record TaskCreatedDomainEvent(Guid Id, Guid TaskListId, Guid TaskId) : DomainEvent(Id);
}
