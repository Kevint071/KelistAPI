using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(Guid Id, string Name, string LastName, string Email, string? Password = null) : IRequest<ErrorOr<UserDTO>>;
}
