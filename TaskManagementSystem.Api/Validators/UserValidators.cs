using FluentValidation;
using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name cannot be empty.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name cannot be empty.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email format is required.");

        // Validates standard international phone characters: digits, plus, minus, spaces, parentheses
        RuleFor(x => x.Phone)
            .Matches(@"^[\d\+\-\(\)\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number can only contain numbers, spaces, and valid symbols (+, -, ()). Letters are not allowed.");
    }
}

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name cannot be empty.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name cannot be empty.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email format is required.");

        RuleFor(x => x.Phone)
            .Matches(@"^[\d\+\-\(\)\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number can only contain numbers, spaces, and valid symbols (+, -, ()). Letters are not allowed.");
    }
}
