using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Data.Interfaces;
using Application.Data.Repositories;
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

            // Eliminar la TaskList usando el método del repositorio
            _userRepository.DeleteTaskList(command.UserId, command.TaskListId);

            // Guardar los cambios
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
