using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using ErrorOr;
using MediatR;
using Domain.DomainErrors;

namespace Application.AuthUsers.Command.RegisterUser
{
    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<ErrorOr<UserDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email)) return Errors.User.DuplicatedEmail;

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(request.Email, request.Name, request.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastname, email) = validationResult.Value;
            var password = _passwordService.HashPassword(request.Password);

            var userDto = new UserDTO(
                Guid.NewGuid(),
                name.Value,
                lastname.Value,
                email.Value,
                password
            );

            _userRepository.Add(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return userDto;
        }
    }
}
