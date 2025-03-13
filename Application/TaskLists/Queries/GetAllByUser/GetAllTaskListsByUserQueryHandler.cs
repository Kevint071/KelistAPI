using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Queries.GetAllByUser
{
    internal sealed class GetAllTaskListsByUserQueryHandler : IRequestHandler<GetAllTaskListsByUserQuery, ErrorOr<List<TaskListDTO>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllTaskListsByUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<List<TaskListDTO>>> Handle(GetAllTaskListsByUserQuery query, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(query.UserId);

            if (userDto == null)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            return userDto.TaskLists;
        }
    }
}
