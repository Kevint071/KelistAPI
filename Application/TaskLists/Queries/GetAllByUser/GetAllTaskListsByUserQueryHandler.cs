using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;

namespace Application.TaskLists.Queries.GetAllByUser
{
    internal sealed class GetAllTaskListsByUserQueryHandler : IRequestHandler<GetAllTaskListsByUserQuery, ErrorOr<IReadOnlyList<TaskListDTO>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllTaskListsByUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ErrorOr<IReadOnlyList<TaskListDTO>>> Handle(GetAllTaskListsByUserQuery query, CancellationToken cancellationToken)
        {
            var userDto = await _userRepository.GetByIdAsync(query.UserId);
            if (userDto == null) return Errors.User.NotFound;

            return userDto.TaskLists;
        }
    }
}
