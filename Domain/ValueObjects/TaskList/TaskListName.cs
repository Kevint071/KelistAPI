using ErrorOr;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects.TaskList
{
    public partial record TaskListName
    {
        private const string SpacePattern = @"\s+";
        public string Value { get; init; }

        private TaskListName(string value) => Value = value;

        public static ErrorOr<TaskListName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("User.Name", "El nombre no puede estar vacío.");
            }

            var normalizedValue = NormalizeName(value);

            if (normalizedValue.Length > 100)
            {
                return Error.Validation("User.Name", "El nombre debe tener menos de 100 caracteres.");
            }

            return new TaskListName(normalizedValue);
        }

        private static string NormalizeName(string value)
        {
            return SpaceRegex().Replace(value, " ").Trim();
        }

        [GeneratedRegex(SpacePattern)]
        private static partial Regex SpaceRegex();
    }
}