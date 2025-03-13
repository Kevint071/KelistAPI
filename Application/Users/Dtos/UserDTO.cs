using Application.TaskLists.Dtos;

namespace Application.Users.Dtos
{
    public record UserDTO(
           Guid Id,
           string Name,
           string LastName,
           string Email,
           List<TaskListDTO>? TaskLists = null
       )
    {
        private UserDTO() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, null) { }
        public List<TaskListDTO> TaskLists { get; init; } = TaskLists ?? [];
        public string FullName => $"{Name} {LastName}";
    }
}
