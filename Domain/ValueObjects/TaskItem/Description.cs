using ErrorOr;

namespace Domain.ValueObjects.TaskItem
{
    public partial record Description
    {
        public string Value { get; init; }

        private Description(string value) => Value = value;

        public static ErrorOr<Description> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("TaskItem.Description", "La descripción no puede estar vacía.");
            }
            return new Description(value);
        }
    }
}