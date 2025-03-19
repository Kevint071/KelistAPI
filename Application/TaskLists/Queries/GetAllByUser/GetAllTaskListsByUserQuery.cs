using Application.TaskLists.Dtos;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Queries.GetAllByUser
{
    public record GetAllTaskListsByUserQuery(Guid UserId) : IRequest<ErrorOr<IReadOnlyList<TaskListDTO>>>;
}
