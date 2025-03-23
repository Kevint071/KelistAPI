using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Common.Validations
{
    public static partial class PasswordValidationRules
    {
        public static IRuleBuilderOptions<T, string> ApplyPasswordRules<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no debe exceder los 100 caracteres.")
                .Must(ContainUppercase).WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Must(ContainLowercase).WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Must(ContainNumber).WithMessage("La contraseña debe contener al menos un número.")
                .Must(ContainSpecialCharacter).WithMessage("La contraseña debe contener al menos un carácter especial.");
        }

        private static bool ContainUppercase(string password)
        {
            return UppercaseRegex().IsMatch(password);
        }

        private static bool ContainLowercase(string password)
        {
            return LowercaseRegex().IsMatch(password);
        }

        private static bool ContainNumber(string password)
        {
            return NumberRegex().IsMatch(password);
        }

        private static bool ContainSpecialCharacter(string password)
        {
            return SpecialCharacterRegex().IsMatch(password);
        }

        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex UppercaseRegex();
        [GeneratedRegex(@"[a-z]")]
        private static partial Regex LowercaseRegex();
        [GeneratedRegex(@"\d")]
        private static partial Regex NumberRegex();
        [GeneratedRegex(@"[!@#$%^&*(),.?""':{}|<>]")]
        private static partial Regex SpecialCharacterRegex();
    }
}
