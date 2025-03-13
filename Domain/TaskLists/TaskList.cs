using Domain.Tasks;

namespace Domain.TaskLists
{
    public sealed class TaskList
    {
        public TaskList(TaskListId id, string name)
        {
            Id = id;
            Name = name;
        }

        private TaskList() { }

        public TaskListId Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public List<TaskItem> TaskItems { get; private set; } = [];

        public void AddTaskItem(TaskItem taskItem)
        {
            TaskItems.Add(taskItem);
        }
    }
}