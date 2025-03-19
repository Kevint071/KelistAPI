using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.UpdateTaskList
{
    internal sealed class UpdateTaskListCommandHandler : IRequestHandler<UpdateTaskListCommand, ErrorOr<TaskListDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskListCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<TaskListDTO>> Handle(UpdateTaskListCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null) return Errors.User.NotFound;

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null) return Errors.TaskList.NotFound;

            var validationResult = ValueObjectValidator.ValidateTaskListValueObjects(command.Name);
            if (validationResult.IsError) return validationResult.Errors;

            var name = validationResult.Value;

            var updatedTaskListDto = new TaskListDTO
            {
                Id = command.TaskListId,
                TaskListName = name.Value,
                TaskItems = taskListDto.TaskItems
            };

            _userRepository.UpdateTaskList(command.UserId, updatedTaskListDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return updatedTaskListDto;
        }
    }
}
