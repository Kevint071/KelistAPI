using Application.TaskLists.Dtos;

namespace Application.Tasks.Dtos
{
    public class TaskListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public List<TaskItemDTO> TaskItems { get; set; } = [];
    }
}