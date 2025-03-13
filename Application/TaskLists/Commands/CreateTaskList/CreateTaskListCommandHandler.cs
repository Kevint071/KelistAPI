using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Users.Services;
using Domain.TaskLists;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.CreateTaskList
{
    internal sealed class CreateTaskListCommandHandler : IRequestHandler<CreateTaskListCommand, ErrorOr<Unit>>
    {
        private readonly UserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskListCommandHandler(UserService userService, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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

            var taskListDto = new TaskListDTO
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                TaskItems = [] // Inicializar la lista vacía
            };
            _userRepository.AddTaskListToUser(command.UserId, taskListDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
