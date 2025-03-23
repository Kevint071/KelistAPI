using Application.TaskLists.Dtos;

namespace Application.Users.Dtos
{
    public class UserDTO
    {
        public Guid Id { get; private set; }
        public string PersonName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{PersonName} {LastName}";
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public List<TaskListDTO> TaskLists { get; init; } = [];

        public UserDTO(Guid id, string personName, string lastName, string email, string passwordHash, string? refreshToken = null, DateTime? refreshTokenExpiryTime = null, List<TaskListDTO>? taskLists = null)
        {
            Id = id;
            PersonName = personName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
            TaskLists = taskLists ?? [];
        }

        // Constructor privado para EF Core
        private UserDTO() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }

        public void UpdateProfile(string name, string lastName, string email)
        {
            PersonName = name;
            LastName = lastName;
            Email = email;
        }

        public void UpdateTokens(string? refreshToken, DateTime? refreshTokenExpiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
        }
    }

    public record RegisterUserDto(string Name, string LastName, string Email, string Password);
    public record LoginUserDto(string Email, string Password);
    public record TokenResponseDto(string AccessToken, string RefreshToken);
    public record RefreshTokenRequestDto(Guid UserId, string RefreshToken);
}