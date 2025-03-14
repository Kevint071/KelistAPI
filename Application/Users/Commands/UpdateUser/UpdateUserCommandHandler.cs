using Application.Common;
using Application.Data.Repositories;
using Application.Users.Services;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUser
{
    internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;

        public UpdateUserCommandHandler(IUserRepository userRepository, UserService userService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<Unit>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            if (!await _userRepository.ExistsAsync(command.Id))
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastName, email) = validationResult.Value;
            var user = new User(new UserId(command.Id), name, lastName, email);

            await _userService.UpdateAsync(user, cancellationToken);
            return Unit.Value;
        }
    }
}
