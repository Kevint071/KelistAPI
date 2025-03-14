using FluentValidation;

namespace Application.TaskLists.Commands.UpdateTaskList
{
    public class CreateTaskListCommandValidator : AbstractValidator<UpdateTaskListCommand>
    {
        public CreateTaskListCommandValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
