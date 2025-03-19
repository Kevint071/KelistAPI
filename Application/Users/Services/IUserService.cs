using Application.Users.Dtos;
using Domain.Users;

namespace Application.Users.Services
{
    public interface IUserService
    {
        Task<UserDTO> AddAsync(User user, CancellationToken cancellationToken);
        Task<UserDTO> UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(UserDTO userDto, CancellationToken cancellationToken);
    }
}
