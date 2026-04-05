using FluentValidation.TestHelper;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Validators;

namespace TaskManagementSystem.Tests.Validators;

public class TagValidatorTests
{
    private readonly TagCreateDtoValidator _validator;

    public TagValidatorTests()
    {
        _validator = new TagCreateDtoValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateTag_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var model = new TagCreateDto(name);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateTag_ShouldHaveError_WhenNameTooLong()
    {
        var model = new TagCreateDto(new string('A', 101));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateTag_ShouldNotHaveError_WhenValid()
    {
        var model = new TagCreateDto("Critical");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
