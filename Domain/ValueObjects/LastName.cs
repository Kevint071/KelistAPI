using System.Globalization;
using ErrorOr;

namespace Domain.ValueObjects
{
    public partial record LastName
    {
        public string Value { get; init; }

        private LastName(string value) => Value = value;

        public static ErrorOr<LastName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("User.LastName", "El apellido no puede estar vacío.");
            }

            var normalizedValue = NormalizeLastName(value);

            if (value.Length > 50)
            {
                return Error.Validation("User.LastName", "El apellido debe tener menos de 50 caracteres.");
            }

            if (!normalizedValue.All(c => char.IsLetter(c) || c == ' '))
            {
                return Error.Validation("User.LastName", "El apellido solo puede contener letras.");
            }

            return new LastName(normalizedValue);
        }

        private static string NormalizeLastName(string value)
        {
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(word =>
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower())));
        }
    }
}
