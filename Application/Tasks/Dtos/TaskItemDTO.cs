namespace Application.Tasks.Dtos
{
    public class TaskItemDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = default!;
        public bool IsCompleted { get; set; }
    }
}