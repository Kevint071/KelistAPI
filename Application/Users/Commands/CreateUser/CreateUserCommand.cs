using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.CreateUser
{
    public record CreateUserCommand(string Name, string LastName, string Email, string Password) : IRequest<ErrorOr<UserDTO>>;
}
