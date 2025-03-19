using ErrorOr;
using Domain.ValueObjects.User;
using Domain.ValueObjects.TaskList;


namespace Application.Common
{
    public static class ValueObjectValidator
    {
        public static ErrorOr<(PersonName, LastName, Email)> ValidateUserValueObjects(string email, string name, string lastName)
        {
            var nameResult = PersonName.Create(name);
            if (nameResult.IsError) return nameResult.Errors;

            var lastNameResult = LastName.Create(lastName);
            if (lastNameResult.IsError) return lastNameResult.Errors;

            var emailResult = Email.Create(email);
            if (emailResult.IsError) return emailResult.Errors;

            return (nameResult.Value, lastNameResult.Value, emailResult.Value);
        }

        public static ErrorOr<TaskListName> ValidateTaskListValueObjects(string name)
        {
            var nameResult = TaskListName.Create(name);
            if (nameResult.IsError) return nameResult.Errors;

            return nameResult.Value;
        }
    }
}
