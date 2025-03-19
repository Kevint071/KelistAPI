using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Services;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.DeleteUser
{
    internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByIdAsync(command.Id) is not UserDTO userDto) return Errors.User.NotFound;

            await _userService.DeleteAsync(userDto, cancellationToken);

            return Unit.Value;
        }
    }
}
