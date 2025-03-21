using Application.Users.Dtos;
using ErrorOr;

namespace Application.Users.Services
{
    public interface IAuthService
    {
        Task<ErrorOr<UserDTO>> RegisterAsync(RegisterUserDto request);
        Task<ErrorOr<TokenResponseDto>> LoginAsync(LoginUserDto request);
        Task<ErrorOr<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
