using Application.Tasks.Dtos;

namespace Application.TaskLists.Dtos
{
    public class TaskListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public List<TaskItemDTO> TaskItems { get; set; } = [];
    }
}