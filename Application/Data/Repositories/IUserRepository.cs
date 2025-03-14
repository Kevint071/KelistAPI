using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;

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
        void DeleteTaskList(Guid userId, Guid taskListId);

        void AddTaskItemToTaskList(Guid userId, Guid taskListId, TaskItemDTO taskItemDTO);
        void UpdateTaskItem(Guid userId, Guid taskListId, TaskItemDTO taskItemDTO);
        void DeleteTaskItem(Guid userId, Guid taskListId, Guid taskItemId);
    }
}