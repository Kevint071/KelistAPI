using System.Text.RegularExpressions;
using ErrorOr;

namespace Domain.ValueObjects
{
    public partial record Email
    {
        private const string Pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public string Value { get; init; }

        private Email(string value) => Value = value;

        public static ErrorOr<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.Validation("User.Email", "Email no puede ser vacio"); 
            }

            if (!EmailRegex().IsMatch(value))
            {
                return Error.Validation("User.Email", "Estructura de Email inválida");
            }

            if (value.Length > 255)
            {
                return Error.Validation("User.Email", "Email no puede ser mayor a 255 caracteres");
            }

            return new Email(value);
        }

        // Método que asume que el valor proviene de la base de datos y es válido.
        public static Email FromPersistence(string value) => new(value);

        [GeneratedRegex(Pattern)]
        private static partial Regex EmailRegex();
    }
}
