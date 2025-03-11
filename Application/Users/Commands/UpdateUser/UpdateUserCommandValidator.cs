using FluentValidation;

namespace Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty();

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(r => r.LastName)
                .NotEmpty()
                .MaximumLength(50)
                .WithName("Last Name");

            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);
        }
    }
}
