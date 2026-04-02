using FluentValidation;
using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Validators;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DueDate).GreaterThan(System.DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DueDate).GreaterThan(System.DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}
