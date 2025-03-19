using Domain.Events.User;
using Domain.TaskLists;
using Domain.Users;
using Domain.ValueObjects.TaskList;
using Domain.ValueObjects.User;

namespace Kelist.Tests.Unit.Domain.Users
{
    public class UserTests
    {
        private readonly UserId _userId = new(Guid.NewGuid());
        private readonly PersonName _name = PersonName.Create("  John ").Value;
        private readonly LastName _lastName = LastName.Create("   Doe   ").Value;
        private readonly Email _email = Email.Create("john.doe@example.com").Value;

        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Act
            var user = new User(_userId, _name, _lastName, _email);

            // Assert
            Assert.Equal(_userId, user.Id);
            Assert.Equal(_name, user.PersonName);
            Assert.Equal(_lastName, user.LastName);
            Assert.Equal("John Doe", user.FullName);
            Assert.Equal(_email, user.Email);
            Assert.Empty(user.TaskLists);
        }

        [Fact]
        public void NotifyCreate_RaisesUserCreatedEvent()
        {
            // Arrange
            var user = new User(_userId, _name, _lastName, _email);

            // Act
            user.NotifyCreate();

            // Assert
            var events = user.GetDomainEvents();
            Assert.Single(events);
            var createdEvent = Assert.IsType<UserCreatedEvent>(events.First());
            Assert.Equal(_userId.Value, createdEvent.UserId);
            Assert.Equal("John Doe", createdEvent.FullName);
            Assert.Equal("john.doe@example.com", createdEvent.Email);
        }

        [Fact]
        public void NotifyUpdate_RaisesUserUpdateEvent()
        {
            // Arrange
            var user = new User(_userId, _name, _lastName, _email);

            // Act
            user.NotifyUpdate();

            // Assert
            var events = user.GetDomainEvents();
            Assert.Single(events);
            var updateEvent = Assert.IsType<UserUpdateEvent>(events.First());
            Assert.Equal(_userId.Value, updateEvent.UserId);
            Assert.Equal("John Doe", updateEvent.FullName);
            Assert.Equal("john.doe@example.com", updateEvent.Email);
        }

        [Fact]
        public void NotifyDelete_RaisesUserDeletedEvent()
        {
            // Arrange
            var user = new User(_userId, _name, _lastName, _email);

            // Act
            user.NotifyDelete();

            // Assert
            var events = user.GetDomainEvents();
            Assert.Single(events);
            var deleteEvent = Assert.IsType<UserDeletedEvent>(events.First());
            Assert.Equal(_userId.Value, deleteEvent.UserId);
            Assert.Equal("John Doe", deleteEvent.FullName);
        }
    }
}
