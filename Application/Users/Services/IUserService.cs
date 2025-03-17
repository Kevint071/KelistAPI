using Application.Users.Dtos;
using Domain.Users;

namespace Application.Users.Services
{
    public interface IUserService
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(UserDTO userDto, CancellationToken cancellationToken);
    }
}