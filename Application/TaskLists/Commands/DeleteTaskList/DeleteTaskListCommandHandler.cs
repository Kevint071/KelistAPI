using Application.Data.Interfaces;
using Application.Data.Repositories;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Commands.DeleteTaskList
{
    internal sealed class DeleteTaskListCommandHandler : IRequestHandler<DeleteTaskListCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskListCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteTaskListCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(command.UserId);
            if (userDto == null) return Errors.User.NotFound;

            var taskListDto = userDto.TaskLists.FirstOrDefault(tl => tl.Id == command.TaskListId);
            if (taskListDto == null) return Errors.TaskList.NotFound;

            _userRepository.DeleteTaskList(command.UserId, command.TaskListId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
