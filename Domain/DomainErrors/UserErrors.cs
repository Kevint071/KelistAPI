using ErrorOr;

namespace Domain.DomainErrors
{
    public static partial class Errors
    {
        public static class User
        {
            public static readonly Error NotFound = Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
        }
    }
}
