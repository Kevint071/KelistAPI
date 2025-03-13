using System.Globalization;
using ErrorOr;

namespace Domain.ValueObjects
{
    public partial record Name
    {
        public string Value { get; init; }

        private Name(string value) => Value = value;

        public static ErrorOr<Name> Create(string value)
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

            return new Name(normalizedValue);
        }

        private static string NormalizeName(string value)
        {
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(word =>
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower())));
        }
    }
}
