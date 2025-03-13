using Domain.Events.User;
using Domain.Primitives;
using Domain.TaskLists;
using Domain.ValueObjects;

namespace Domain.Users
{
    public sealed class User : AggregateRoot
    {
        public User(UserId id, Name name, LastName lastname, Email email)
        {
            Id = id;
            Name = name;
            LastName = lastname;
            Email = email;
        }

        private User() { }

        public UserId Id { get; private set; } = default!;
        public Name Name { get; private set; } = default!;
        public LastName LastName { get; private set; } = default!;
        public string FullName => $"{Name.Value} {LastName.Value}";
        public Email Email { get; private set; } = default!;
        public List<TaskList> TaskLists { get; private set; } = [];

        public void AddTaskList(TaskList taskList)
        {
            TaskLists.Add(taskList);
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
