using Application.Users.Common;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetAll
{
    public record GetAllUsersQuery() : IRequest<ErrorOr<IReadOnlyList<UserResponse>>>;
}
