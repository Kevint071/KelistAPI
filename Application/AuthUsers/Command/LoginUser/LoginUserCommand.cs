using Application.AuthUsers.Dtos;
using ErrorOr;
using MediatR;

namespace Application.AuthUsers.Command.LoginUser
{
    public record LoginUserCommand(string Email, string Password) : IRequest<ErrorOr<TokenResponseDTO>>;
}
