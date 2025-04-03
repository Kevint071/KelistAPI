using System.Security.Claims;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;
using Application.Common.Mappers;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Command.LoginUser
{
    internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ErrorOr<TokenResponseDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public LoginUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<ErrorOr<TokenResponseDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByEmailAsync(request.Email);
            if (userDto == null || !_passwordService.VerifyPassword(request.Password, userDto.PasswordHash)) return Errors.User.InvalidCredentials;

            var user = UserMapper.ToDomain(userDto);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new(ClaimTypes.Email, user.Email.Value),
                new(ClaimTypes.Role, user.Role)
            };

            string accessToken = _tokenService.CreateJwtToken(claims);
            string refreshToken = _tokenService.GenerateRefreshToken();

            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            userDto.UpdateTokens(user.RefreshToken, user.RefreshTokenExpiryTime);

            _userRepository.Update(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new TokenResponseDTO(accessToken, refreshToken);
        }
    }
}
