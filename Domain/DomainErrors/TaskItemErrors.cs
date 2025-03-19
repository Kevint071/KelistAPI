using ErrorOr;

namespace Domain.DomainErrors
{
    public static partial class Errors
    {
        public static class TaskItems
        {
            public static readonly Error NotFound = Error.NotFound("TaskItem.NotFound", "The task item with the provided Id was not found.");
        }
    }
}
