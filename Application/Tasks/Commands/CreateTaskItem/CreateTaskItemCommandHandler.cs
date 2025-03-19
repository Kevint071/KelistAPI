using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;
using Domain.DomainErrors;

namespace Application.Tasks.Commands.CreateTaskItem
{
    internal sealed class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, ErrorOr<TaskItemDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskItemCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<TaskItemDTO>> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null) return Errors.User.NotFound;

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null) return Errors.TaskList.NotFound;

            var taskItemDto = new TaskItemDTO
            {
                Id = Guid.NewGuid(),
                Description = command.Description,
                IsCompleted = false
            };

            _userRepository.AddTaskItemToTaskList(command.UserId, command.TaskListId, taskItemDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return taskItemDto;
        }
    }
}
