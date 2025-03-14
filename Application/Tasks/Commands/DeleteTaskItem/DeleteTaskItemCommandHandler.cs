using Application.Data.Interfaces;
using Application.Data.Repositories;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Commands.DeleteTaskItem
{
    internal sealed class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskItemCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null)
            {
                return Error.NotFound("TaskList.NotFound", "The task list with the provided Id was not found.");
            }

            var taskItemDto = taskListDto.TaskItems.FirstOrDefault(ti => ti.Id == command.TaskItemId);
            if (taskItemDto == null)
            {
                return Error.NotFound("TaskItem.NotFound", "The task item with the provided Id was not found.");
            }

            _userRepository.DeleteTaskItem(command.UserId, command.TaskListId, command.TaskItemId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
