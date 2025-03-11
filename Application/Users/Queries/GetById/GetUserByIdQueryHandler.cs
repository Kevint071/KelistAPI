using Application.Data.Repositories;
using Application.Users.Common;
using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetById
{
    public record GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ErrorOr<UserResponse>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByIdAsync(query.Id) is not UserDTO user)
            {
                return Error.NotFound("User.NotFound", "The user with the provide Id was not found.");
            }

            return new UserResponse(user.Id, user.FullName, user.Email);
        }
    }
}
