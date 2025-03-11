using Application.Users.Common;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<ErrorOr<UserResponse>>;
}
