using Domain.Tasks;
using Domain.ValueObjects.TaskList;

namespace Domain.TaskLists
{
    public sealed class TaskList
    {
        public TaskList(TaskListId id, TaskListName name)
        {
            Id = id;
            TaskListName = name;
        }

        private TaskList() { }

        public TaskListId Id { get; private set; } = default!;
        public TaskListName TaskListName { get; private set; } = default!;
        public List<TaskItem> TaskItems { get; private set; } = [];
    }
}
