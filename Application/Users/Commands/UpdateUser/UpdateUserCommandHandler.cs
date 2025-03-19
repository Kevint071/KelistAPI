using Application.Common;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Services;
using Domain.DomainErrors;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUser
{
    internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<UserDTO>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            if (!await _userRepository.ExistsAsync(command.Id)) return Errors.User.NotFound;

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastName, email) = validationResult.Value;
            var user = new User(new UserId(command.Id), name, lastName, email);

            var userDto = await _userService.UpdateAsync(user, cancellationToken);
            return userDto;
        }
    }
}
