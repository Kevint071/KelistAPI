using Application.Data.Repositories;
using Application.Data.Interfaces;
using Domain.Users;
using ErrorOr;
using MediatR;
using Application.Common;
using Application.Users.Services;
using Application.Users.Dtos;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserDTO>>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<UserDTO>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastname, email) = validationResult.Value;
            var user = new User(new UserId(Guid.NewGuid()), name, lastname, email);

            var userDto = await _userService.AddAsync(user, cancellationToken);
            return userDto;
        }
    }
}
