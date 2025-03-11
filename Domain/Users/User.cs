using Domain.Events.User;
using Domain.Primitives;
using Domain.TaskLists;
using Domain.ValueObjects;

namespace Domain.Users
{
    public sealed class User : AggregateRoot
    {
        public User(UserId id, string name, string lastname, Email email)
        {
            Id = id;
            Name = name;
            LastName = lastname;
            Email = email;
        }

        private User() { }

        public UserId Id { get; private set; } = default!;
        public string? Name { get; private set; }
        public string? LastName { get; private set; }
        public string FullName => $"{Name} {LastName}";
        public Email Email { get; private set; } = default!;
        public List<TaskList> TaskLists { get; private set; } = [];

        public void AddTaskList(TaskList taskList)
        {
            TaskLists.Add(taskList);
        }

        public void NotifyCreate()
        {
            Raise(new UserCreatedEvent(Id, FullName, Email.Value));
        }

        public void NotifyUpdate()
        {
            Raise(new UserUpdateEvent(Id, FullName, Email.Value));
        }

        public void NotifyDelete()
        {
            Raise(new UserDeletedEvent(Id, FullName));
        }
    }
}
