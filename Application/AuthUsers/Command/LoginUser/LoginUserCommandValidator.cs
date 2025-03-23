using FluentValidation;

namespace Application.AuthUsers.Command.LoginUser
{
    public partial class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Se requiere un email válido.")
                .MaximumLength(255).WithMessage("El email no debe exceder los 255 caracteres.");
        }
    }
}