using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.AuthUsers.Command.LoginUser
{
    public record LoginUserCommand(string Email, string Password) : IRequest<ErrorOr<TokenResponseDto>>;
}
