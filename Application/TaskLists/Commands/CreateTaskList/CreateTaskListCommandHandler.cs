using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.CreateTaskList
{
    internal sealed class CreateTaskListCommandHandler : IRequestHandler<CreateTaskListCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskListCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(CreateTaskListCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);

            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var validationResult = ValueObjectValidator.ValidateTaskListValueObjects(command.Name);
            if (validationResult.IsError) return validationResult.Errors;

            var name = validationResult.Value;

            var taskListDto = new TaskListDTO
            {
                Id = Guid.NewGuid(),
                TaskListName = name.Value,
                TaskItems = [] // Inicializar la lista vacía
            };
            _userRepository.AddTaskListToUser(command.UserId, taskListDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
