using FluentValidation;

namespace Application.Tasks.Commands.UpdateTaskItem
{
    public class UpdateTaskItemCommandValidator : AbstractValidator<UpdateTaskItemCommand>
    {
        public UpdateTaskItemCommandValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .MaximumLength(600);

            RuleFor(r => r.IsCompleted)
                .NotNull();
        }
    }
}
