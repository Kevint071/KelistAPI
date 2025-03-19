using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Commands.UpdateTaskItem
{
    public record UpdateTaskItemCommand(Guid UserId, Guid TaskListId, Guid TaskItemId, string Description, bool IsCompleted) : IRequest<ErrorOr<TaskItemDTO>>;
}
