using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IValidator<TaskCreateDto> _createValidator;
    private readonly IValidator<TaskUpdateDto> _updateValidator;

    public TasksController(
        ITaskService taskService,
        IValidator<TaskCreateDto> createValidator,
        IValidator<TaskUpdateDto> updateValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _taskService.GetTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _taskService.GetTaskAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto taskDto)
    {
        var validationResult = await _createValidator.ValidateAsync(taskDto);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var createdTask = await _taskService.CreateTaskAsync(taskDto);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto taskDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(taskDto);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var success = await _taskService.UpdateTaskAsync(id, taskDto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var success = await _taskService.DeleteTaskAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
