using Application.Data.Repositories;
using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Queries.GetAllByTaskListId
{
    internal sealed class GetAllTaskItemsByTaskListQueryHandler : IRequestHandler<GetAllTaskItemsByTaskListQuery, ErrorOr<List<TaskItemDTO>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllTaskItemsByTaskListQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<List<TaskItemDTO>>> Handle(GetAllTaskItemsByTaskListQuery query, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(query.UserId);

            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == query.TaskListId);
            if (taskListDto == null)
            {
                return Error.NotFound("TaskList.NotFound", "The task list with the provided Id was not found.");
            }

            return taskListDto.TaskItems;
        }
    }
}
