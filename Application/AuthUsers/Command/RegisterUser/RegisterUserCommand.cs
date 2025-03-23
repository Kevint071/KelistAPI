using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.AuthUsers.Command.RegisterUser
{
    public record RegisterUserCommand (string Name, string LastName, string Email, string Password) : IRequest<ErrorOr<UserDTO>>;
}
