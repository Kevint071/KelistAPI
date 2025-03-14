using Application.TaskLists.Dtos;
using Application.Users.Dtos;
using Domain.Users;

namespace Application.Data.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserDTO>> GetAll();
        Task<UserDTO?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        void Add(UserDTO user);
        void Delete(UserDTO user);
        void Update(UserDTO user);

        void AddTaskListToUser(Guid userId, TaskListDTO taskListDTO);
        void UpdateTaskList(Guid userId, TaskListDTO taskListDTO);
        void DeleteTaskList(Guid userId, Guid TaskListId);
    }
}