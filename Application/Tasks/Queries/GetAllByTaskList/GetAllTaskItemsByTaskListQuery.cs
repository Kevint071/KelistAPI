using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Queries.GetAllByTaskList
{
    public record GetAllTaskItemsByTaskListQuery(Guid UserId, Guid TaskListId) : IRequest<ErrorOr<IReadOnlyList<TaskItemDTO>>>;
}
