using FluentValidation;
using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Validators;

public class TagCreateDtoValidator : AbstractValidator<TagCreateDto>
{
    public TagCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name cannot be empty.")
            .MaximumLength(100).WithMessage("Tag name cannot exceed 100 characters.");
    }
}
