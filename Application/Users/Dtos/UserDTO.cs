using Application.TaskLists.Dtos;

namespace Application.Users.Dtos
{
    public record UserDTO(
           Guid Id,
           string PersonName,
           string LastName,
           string Email,
           string PasswordHash,
           string? RefreshToken = null,
           DateTime? RefreshTokenExpiryTime = null,
           List<TaskListDTO>? TaskLists = null
       )
    {
        private UserDTO() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null, null) { }
        public List<TaskListDTO> TaskLists { get; init; } = TaskLists ?? [];
        public string FullName => $"{PersonName} {LastName}";
        public string? RefreshToken { get; set; } = RefreshToken;
        public DateTime? RefreshTokenExpiryTime { get; set; } = RefreshTokenExpiryTime;
    }

    public record RegisterUserDto(string Name, string LastName, string Email, string Password);
    public record LoginUserDto(string Email, string Password);
    public record TokenResponseDto(string AccessToken, string RefreshToken);
    public record RefreshTokenRequestDto(Guid UserId, string RefreshToken);
}
