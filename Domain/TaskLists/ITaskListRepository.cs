namespace Domain.TaskLists
{
    public interface ITaskListRepository
    {
        Task<List<TaskList>> GetAllAsync();
        Task<TaskList?> GetByIdAsync(TaskListId id);
        Task<bool> ExistsAsync(TaskListId id);
        void Add(TaskList taskList);
        void Update(TaskList taskList);
        void Delete(TaskList taskList);
    }

}
