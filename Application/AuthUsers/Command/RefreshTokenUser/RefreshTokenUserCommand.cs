using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.AuthUsers.Command.RefreshTokenUser
{
    public record RefreshTokenUserCommand(Guid UserId, string RefreshToken) : IRequest<ErrorOr<TokenResponseDto>>;
}
