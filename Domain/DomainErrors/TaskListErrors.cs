using ErrorOr;

namespace Domain.DomainErrors
{
    public static partial class Errors
    {
        public static class TaskList
        {
            public static readonly Error NotFound = Error.NotFound("TaskList.NotFound", "The task list with the provided Id was not found.");
        }
    }
}
