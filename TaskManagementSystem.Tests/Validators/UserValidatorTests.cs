using FluentValidation.TestHelper;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Validators;

namespace TaskManagementSystem.Tests.Validators;

public class UserValidatorTests
{
    private readonly UserCreateDtoValidator _createValidator;
    private readonly UserUpdateDtoValidator _updateValidator;

    public UserValidatorTests()
    {
        _createValidator = new UserCreateDtoValidator();
        _updateValidator = new UserUpdateDtoValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateUser_ShouldHaveError_WhenFirstNameIsEmpty(string firstName)
    {
        var model = new UserCreateDto(firstName, "LName", "test@test.com", "12345");
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@test.com")]
    public void CreateUser_ShouldHaveError_WhenEmailIsInvalid(string email)
    {
        var model = new UserCreateDto("FName", "LName", email, "12345");
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("123-abc")]
    [InlineData("+1 (555) 0101a")]
    [InlineData("phone!")]
    public void CreateUser_ShouldHaveError_WhenPhoneContainsLetters(string phone)
    {
        var model = new UserCreateDto("FName", "LName", "test@test.com", phone);
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void CreateUser_ShouldNotHaveError_WhenPhoneIsValid()
    {
        var model = new UserCreateDto("FName", "LName", "test@test.com", "+1 555-0101");
        var result = _createValidator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void UpdateUser_ShouldHaveError_WhenLastNameIsEmpty()
    {
        var model = new UserUpdateDto("FName", "", "test@test.com", "12345");
        var result = _updateValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateUser_ShouldNotHaveError_WhenValid()
    {
        var model = new UserCreateDto("Alice", "Smith", "alice@example.com", "+1-555-0101");
        var result = _createValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
