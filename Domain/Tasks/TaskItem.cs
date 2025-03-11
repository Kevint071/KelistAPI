namespace Domain.Tasks
{
    public sealed class TaskItem
    {
        public TaskItem(TaskItemId id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = false;
        }

        private TaskItem() { }

        public TaskItemId Id { get; private set; } = new TaskItemId(Guid.NewGuid());
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public bool IsCompleted { get; private set; }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }
    }
}
