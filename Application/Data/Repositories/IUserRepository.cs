using Application.Users.Dtos;
using Domain.Users;

namespace Application.Data.Repositories
{
    public interface IUserRepository
    {
        Task<UserDTO?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        void Add(UserDTO user);
        void Delete(UserDTO user);
        void Update(UserDTO user);
    }
}