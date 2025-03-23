using Domain.Events.User;
using Domain.Primitives;
using Domain.TaskLists;
using Domain.ValueObjects.User;

namespace Domain.Users
{
    public sealed class User : AggregateRoot
    {
        public User(UserId id, PersonName name, LastName lastname, Email email, string passwordHash, string? refreshToken = null, DateTime? refreshTokenExpiryTime = null)
        {
            Id = id;
            PersonName = name;
            LastName = lastname;
            Email = email;
            PasswordHash = passwordHash;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
        }

        private User() { }

        public UserId Id { get; private set; } = default!;
        public PersonName PersonName { get; private set; } = default!;
        public LastName LastName { get; private set; } = default!;
        public string FullName => $"{PersonName.Value} {LastName.Value}";
        public Email Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public List<TaskList> TaskLists { get; private set; } = [];

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void NotifyCreate()
        {
            Raise(new UserCreatedEvent(Id.Value, FullName, Email.Value));
        }

        public void NotifyUpdate()
        {
            Raise(new UserUpdateEvent(Id.Value, FullName, Email.Value));
        }

        public void NotifyDelete()
        {
            Raise(new UserDeletedEvent(Id.Value, FullName));
        }
    }
}
