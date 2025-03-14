using System.Globalization;
using ErrorOr;

namespace Domain.ValueObjects.User
{
    public partial record PersonName
    {
        public string Value { get; init; }

        private PersonName(string value) => Value = value;

        public static ErrorOr<PersonName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("User.Name", "El nombre no puede estar vacío.");
            }

            var normalizedValue = NormalizeName(value);

            if (value.Length > 50)
            {
                return Error.Validation("User.Name", "El nombre debe tener menos de 50 caracteres.");
            }

            if (!normalizedValue.All(c => char.IsLetter(c) || c == ' '))
            {
                return Error.Validation("User.Name", "El nombre solo puede contener letras.");
            }

            return new PersonName(normalizedValue);
        }

        private static string NormalizeName(string value)
        {
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(word =>
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower())));
        }
    }
}
