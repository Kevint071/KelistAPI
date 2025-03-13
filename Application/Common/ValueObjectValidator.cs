using ErrorOr;
using Domain.ValueObjects;

namespace Application.Common
{
    public static class ValueObjectValidator
    {
        public static ErrorOr<(Name, LastName, Email)> ValidateUserValueObjects(string email, string name, string lastName)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsError) return nameResult.Errors;

            var lastNameResult = LastName.Create(lastName);
            if (lastNameResult.IsError) return lastNameResult.Errors;

            var emailResult = Email.Create(email);
            if (emailResult.IsError) return emailResult.Errors;

            return (nameResult.Value, lastNameResult.Value, emailResult.Value);
        }
    }
}