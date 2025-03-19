using Domain.ValueObjects.TaskItem;

namespace Domain.Tasks
{
    public sealed class TaskItem
    {
        public TaskItem(TaskItemId id, Description description, bool isCompleted = false)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            IsCompleted = isCompleted;
        }
        private TaskItem() { }

        public TaskItemId Id { get; private set; } = default!;
        public Description Description { get; private set; } = default!;
        public bool IsCompleted { get; private set; }
    }
}
