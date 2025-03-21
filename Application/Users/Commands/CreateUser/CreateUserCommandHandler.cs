using Application.Common;
using Application.Users.Dtos;
using Application.Users.Services;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserDTO>>
    {
        private readonly IAuthService _authService;

        public CreateUserCommandHandler(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<ErrorOr<UserDTO>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var registerDto = new RegisterUserDto(command.Name, command.LastName, command.Email, command.Password);
            var result = await _authService.RegisterAsync(registerDto);

            return result;
        }
    }
}