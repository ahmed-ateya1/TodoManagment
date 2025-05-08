using FluentValidation;
using TodoManagment.Core.Dtos.TodoDto;

namespace TodoManagment.Core.Validators.TodoValidator
{
    public class TodoAddRequestValidator : AbstractValidator<TodoAddRequest>
    {
        public TodoAddRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(100)
                .WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");

            RuleFor(x=>x.Priority)
                .IsInEnum()
                .WithMessage("Priority must be a valid enum value.");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("Priority must be a valid enum value.");

        }
    }
}
