using Application.Common;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Services;
using Domain.DomainErrors;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
            var existingUserDto = await _userRepository.GetByIdAsync(command.Id);
            if (existingUserDto == null) return Errors.User.NotFound;

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastName, email) = validationResult.Value;
            string passwordHash = existingUserDto.PasswordHash;

            // Si se proporciona una nueva contraseña, hashearla
            if (!string.IsNullOrEmpty(command.Password))
            {
                passwordHash = new PasswordHasher<User>().HashPassword(null!, command.Password);
            }

            var user = new User(
                new UserId(command.Id),
                name,
                lastName,
                email,
                passwordHash,
                existingUserDto.RefreshToken,
                existingUserDto.RefreshTokenExpiryTime
            );

            var userDto = await _userService.UpdateAsync(user, cancellationToken);
            return userDto;
        }
    }
}
