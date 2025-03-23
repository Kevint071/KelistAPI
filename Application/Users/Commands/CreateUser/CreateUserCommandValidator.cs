using Application.Common.Validations;
using FluentValidation;

namespace Application.Users.Commands.CreateUser
{
    public partial class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(r => r.LastName)
                .NotEmpty()
                .MaximumLength(50)
                .WithName("Last Name");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Se requiere un email válido.")
                .MaximumLength(255).WithMessage("El email no debe exceder los 255 caracteres.");

            RuleFor(r => r.Password)
                .ApplyPasswordRules();
        }
    }
}
