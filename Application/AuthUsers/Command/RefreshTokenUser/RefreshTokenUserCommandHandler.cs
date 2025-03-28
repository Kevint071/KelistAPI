using System.Security.Claims;
using Application.AuthUsers.Dtos;
using Application.Common.Mappers;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using ErrorOr;
using MediatR;

namespace Application.AuthUsers.Command.RefreshTokenUser
{
    internal sealed class RefreshTokenUserCommandHandler : IRequestHandler<RefreshTokenUserCommand, ErrorOr<TokenResponseDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RefreshTokenUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<ErrorOr<TokenResponseDTO>> Handle(RefreshTokenUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(request.UserId);
            if (userDto == null || userDto.RefreshToken != request.RefreshToken || userDto.RefreshTokenExpiryTime <= DateTime.UtcNow) return Domain.DomainErrors.Errors.User.InvalidRefreshToken;

            var user = UserMapper.ToDomain(userDto);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new(ClaimTypes.Email, user.Email.Value)
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
