using Application.Tasks.Dtos;

namespace Application.Data.Repositories
{
    public interface ITaskListRepository
    {
        Task<TaskListDTO?> GetByIdAsync(Guid id);
        void Add(TaskListDTO taskList);
        //Task<List<TaskList>> GetAllAsync();
        //Task<bool> ExistsAsync(TaskListId id);
        //void Update(TaskListDTO taskList);
        //void Delete(TaskListDTO taskList);
    }

}
