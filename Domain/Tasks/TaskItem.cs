namespace Domain.Tasks
{
    public sealed class TaskItem
    {
        public TaskItem(TaskItemId id, string description, bool isCompleted = false)
        {
            Id = id;
            Description = description;
            IsCompleted = isCompleted;
        }
        private TaskItem() { }

        public TaskItemId Id { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public bool IsCompleted { get; private set; }
    }
}