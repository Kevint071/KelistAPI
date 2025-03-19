using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Tasks.Dtos;
using ErrorOr;
using MediatR;
using Domain.DomainErrors;

namespace Application.Tasks.Commands.UpdateTaskItem
{
    internal sealed class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, ErrorOr<TaskItemDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskItemCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<TaskItemDTO>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null) return Errors.User.NotFound;

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null) return Errors.TaskList.NotFound;

            var taskItemDto = taskListDto.TaskItems.FirstOrDefault(ti => ti.Id == command.TaskItemId);
            if (taskItemDto == null) return Errors.TaskItems.NotFound;

            var updatedTaskItemDto = new TaskItemDTO
            {
                Id = command.TaskItemId,
                Description = command.Description,
                IsCompleted = command.IsCompleted
            };

            _userRepository.UpdateTaskItem(command.UserId, command.TaskListId, updatedTaskItemDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return updatedTaskItemDto;
        }
    }
}
