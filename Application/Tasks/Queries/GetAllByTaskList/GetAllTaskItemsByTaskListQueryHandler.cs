using Application.Data.Repositories;
using Application.Tasks.Dtos;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Queries.GetAllByTaskList
{
    internal sealed class GetAllTaskItemsByTaskListQueryHandler : IRequestHandler<GetAllTaskItemsByTaskListQuery, ErrorOr<IReadOnlyList<TaskItemDTO>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllTaskItemsByTaskListQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<IReadOnlyList<TaskItemDTO>>> Handle(GetAllTaskItemsByTaskListQuery query, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(query.UserId);

            if (userDto == null) return Errors.User.NotFound;

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == query.TaskListId);
            if (taskListDto == null) return Errors.TaskList.NotFound;

            return taskListDto.TaskItems;
        }
    }
}
