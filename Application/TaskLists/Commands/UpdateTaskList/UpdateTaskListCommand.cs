using Application.TaskLists.Dtos;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.UpdateTaskList
{
    public record UpdateTaskListCommand(Guid UserId, Guid TaskListId, string Name) : IRequest<ErrorOr<TaskListDTO>>;
}
