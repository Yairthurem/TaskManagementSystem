using FluentValidation.TestHelper;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Validators;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Tests.Validators;

public class TaskValidatorTests
{
    private readonly TaskCreateDtoValidator _createValidator;
    private readonly TaskUpdateDtoValidator _updateValidator;

    public TaskValidatorTests()
    {
        _createValidator = new TaskCreateDtoValidator();
        _updateValidator = new TaskUpdateDtoValidator();
    }

    [Fact]
    public void CreateTask_ShouldHaveError_WhenTitleIsEmpty()
    {
        var model = new TaskCreateDto("", "Desc", null, TaskPriority.Medium, 1, null);
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateTask_ShouldHaveError_WhenTitleTooLong()
    {
        var model = new TaskCreateDto(new string('A', 101), "Desc", null, TaskPriority.Medium, 1, null);
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateTask_ShouldHaveError_WhenDueDateInPast()
    {
        var model = new TaskCreateDto("Title", "Desc", DateTime.UtcNow.AddDays(-1), TaskPriority.Medium, 1, null);
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void CreateTask_ShouldHaveError_WhenUserIdIsZero()
    {
        var model = new TaskCreateDto("Title", "Desc", null, TaskPriority.Medium, 0, null);
        var result = _createValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void CreateTask_ShouldNotHaveError_WhenValid()
    {
        var model = new TaskCreateDto("Valid Title", "Description", DateTime.UtcNow.AddDays(1), TaskPriority.High, 1, new List<int> { 1, 2 });
        var result = _createValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateTask_ShouldHaveError_WhenTitleIsEmpty()
    {
        var model = new TaskUpdateDto("", "Desc", null, TaskPriority.Medium, 1, null);
        var result = _updateValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void UpdateTask_ShouldHaveError_WhenTagIdIsNegative()
    {
        var model = new TaskUpdateDto("Title", "Desc", null, TaskPriority.Medium, 1, new List<int> { -1 });
        var result = _updateValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("TagIds[0]");
    }
}
