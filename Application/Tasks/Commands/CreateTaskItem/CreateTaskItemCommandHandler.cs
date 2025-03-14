using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Tasks.Commands.CreateTaskItem
{
    internal sealed class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskItemCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            // Obtener el usuario con sus TaskLists
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            // Verificar si la TaskList existe
            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null)
            {
                return Error.NotFound("TaskList.NotFound", "The task list with the provided Id was not found.");
            }

            var taskItemDto = new TaskItemDTO
            {
                Id = Guid.NewGuid(),
                Description = command.Description,
                IsCompleted = false
            };

            _userRepository.AddTaskItemToTaskList(command.UserId, command.TaskListId, taskItemDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
