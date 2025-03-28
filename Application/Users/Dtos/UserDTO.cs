using System.Data;
using Application.TaskLists.Dtos;

namespace Application.Users.Dtos
{
    public class UserDTO(Guid id, string personName, string lastName, string email, string passwordHash, string role = "User", string? refreshToken = null, DateTime? refreshTokenExpiryTime = null, List<TaskListDTO>? taskLists = null)
    {
        public Guid Id { get; private set; } = id;
        public string PersonName { get; private set; } = personName;
        public string LastName { get; private set; } = lastName;
        public string FullName => $"{PersonName} {LastName}";
        public string Email { get; private set; } = email;
        public string PasswordHash { get; private set; } = passwordHash;
        public string Role { get; private set; } = role;
        public string? RefreshToken { get; private set; } = refreshToken;
        public DateTime? RefreshTokenExpiryTime { get; private set; } = refreshTokenExpiryTime;
        public List<TaskListDTO> TaskLists { get; init; } = taskLists ?? [];

        // Constructor privado para EF Core
        private UserDTO() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "User") { }

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
}