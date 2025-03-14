using FluentValidation;

namespace Application.Tasks.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
    {
        public CreateTaskItemCommandValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .MaximumLength(600);

            RuleFor(r => r.IsCompleted)
                .NotNull();
        }
    }
}
