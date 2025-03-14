using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Queries.GetAllByTaskListId
{
    public record GetAllTaskItemsByTaskListQuery(Guid UserId, Guid TaskListId) : IRequest<ErrorOr<List<TaskItemDTO>>>;
}
