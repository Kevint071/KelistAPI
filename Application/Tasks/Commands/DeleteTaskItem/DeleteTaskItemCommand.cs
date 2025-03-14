using ErrorOr;
using MediatR;

namespace Application.Tasks.Commands.DeleteTaskItem
{
    public record DeleteTaskItemCommand(Guid UserId, Guid TaskListId, Guid TaskItemId) : IRequest<ErrorOr<Unit>>;
}
