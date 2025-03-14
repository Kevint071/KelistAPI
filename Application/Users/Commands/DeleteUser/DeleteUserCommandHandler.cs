using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Services;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.DeleteUser
{
    internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;

        public DeleteUserCommandHandler(IUserRepository userRepository, UserService userService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByIdAsync(command.Id) is not UserDTO userDto)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            await _userService.DeleteAsync(userDto, cancellationToken);

            return Unit.Value;
        }
    }
}
