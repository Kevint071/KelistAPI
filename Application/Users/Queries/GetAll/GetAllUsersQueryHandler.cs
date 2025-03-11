using Application.Data.Repositories;
using Application.Users.Common;
using Application.Users.Dtos;
using ErrorOr;
using MediatR;

namespace Application.Users.Queries.GetAll
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<IReadOnlyList<UserResponse>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<IReadOnlyList<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            IReadOnlyList<UserDTO> users = await _userRepository.GetAll();

            return users.Select(user => new UserResponse(
                user.Id,
                user.FullName,
                user.Email)).ToList();
        }
    }
}