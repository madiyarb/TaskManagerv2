using FluentValidation;

namespace TaskManager.Application.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(a=>a.Task).NotNull().WithMessage("Task is required.");
        RuleFor(a=>a.Task.Name).NotEmpty().NotNull().WithMessage("Name is required.");
        RuleFor(a=>a.Task.Phone).NotEmpty().NotNull().WithMessage("Phone is required.");
    }
}