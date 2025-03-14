using ErrorOr;
using MediatR;

namespace Application.Tasks.Commands.CreateTaskItem
{
    public record CreateTaskItemCommand(Guid UserId, Guid TaskListId, string Description, bool IsCompleted): IRequest<ErrorOr<Unit>>;
}
