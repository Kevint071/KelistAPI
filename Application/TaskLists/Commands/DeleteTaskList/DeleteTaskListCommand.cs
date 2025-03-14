using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.DeleteTaskList
{
    public record DeleteTaskListCommand(Guid UserId, Guid TaskListId) : IRequest<ErrorOr<Unit>>;
}
