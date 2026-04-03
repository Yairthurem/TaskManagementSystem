using FluentValidation;
using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Validators;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be a valid positive number.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Priority must be a valid option.");
        RuleFor(x => x.DueDate)
            .Must(date => !date.HasValue || date.Value.Date >= DateTime.UtcNow.Date)
            .WithMessage("Due Date cannot be in the past.");
        RuleForEach(x => x.TagIds).GreaterThan(0).WithMessage("All Tag IDs must be valid positive numbers.").When(x => x.TagIds != null);
    }
}

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User ID must be a valid positive number.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Priority must be a valid option.");
        RuleForEach(x => x.TagIds).GreaterThan(0).WithMessage("All Tag IDs must be valid positive numbers.").When(x => x.TagIds != null);
    }
}
