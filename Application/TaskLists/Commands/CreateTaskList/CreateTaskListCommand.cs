using Application.TaskLists.Dtos;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.CreateTaskList
{
    public record CreateTaskListCommand(Guid UserId, string Name) : IRequest<ErrorOr<TaskListDTO>>;
}
