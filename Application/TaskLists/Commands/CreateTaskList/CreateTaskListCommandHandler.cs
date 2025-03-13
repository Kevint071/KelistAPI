using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Tasks.Dtos;
using Domain.TaskLists;
using Domain.Users;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.CreateTaskList
{
    internal sealed class CreateTaskListCommandHandler : IRequestHandler<CreateTaskListCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskListRepository _taskListRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskListCommandHandler(IUserRepository userRepository, ITaskListRepository taskListRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _taskListRepository = taskListRepository ?? throw new ArgumentNullException(nameof(taskListRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(CreateTaskListCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);

            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(userDto.Email, userDto.Name, userDto.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var user = new User(new UserId(Guid.NewGuid()), Name.Create(userDto.Name).Value, LastName.Create(userDto.LastName).Value, Email.Create(userDto.Email).Value);
            user.TaskLists.AddRange(userDto.TaskLists.Select(tl => new TaskList(
                new TaskListId(tl.Id),
                tl.Name
            )));

            var taskList = new TaskList(new TaskListId(Guid.NewGuid()), command.Name);
            user.AddTaskList(taskList);

            userDto.TaskLists.Add(new TaskListDTO
            {
                Id = taskList.Id.Value,
                Name = taskList.Name
            });

            _userRepository.Update(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
