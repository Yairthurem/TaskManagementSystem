using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync()
    {
        var tasks = await _taskRepository.GetTasksAsync();
        return tasks.Select(MapToResponseDto);
    }

    public async Task<TaskResponseDto?> GetTaskAsync(int id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        return task == null ? null : MapToResponseDto(task);
    }

    public async Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto taskDto)
    {
        var task = new TaskEntity
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            UserId = taskDto.UserId
        };

        if (taskDto.TagIds != null && taskDto.TagIds.Any())
        {
            foreach (var tagId in taskDto.TagIds)
            {
                task.TaskTags.Add(new TaskTag { TagId = tagId });
            }
        }

        var createdTask = await _taskRepository.AddTaskAsync(task);
        return MapToResponseDto(createdTask);
    }

    public async Task<bool> UpdateTaskAsync(int id, TaskUpdateDto taskDto)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null) return false;

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.DueDate = taskDto.DueDate;
        task.Priority = taskDto.Priority;
        task.UserId = taskDto.UserId;

        // Simplified many-to-many update: clear existing and insert new
        task.TaskTags.Clear();
        if (taskDto.TagIds != null && taskDto.TagIds.Any())
        {
            foreach (var tagId in taskDto.TagIds)
            {
                task.TaskTags.Add(new TaskTag { TagId = tagId });
            }
        }

        await _taskRepository.UpdateTaskAsync(task);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null) return false;

        await _taskRepository.DeleteTaskAsync(task);
        return true;
    }

    private static TaskResponseDto MapToResponseDto(TaskEntity task)
    {
        return new TaskResponseDto(
            task.Id,
            task.Title,
            task.Description,
            DateTime.SpecifyKind(task.CreatedAt, DateTimeKind.Utc),
            task.DueDate.HasValue ? DateTime.SpecifyKind(task.DueDate.Value, DateTimeKind.Utc) : null,
            task.Priority,
            task.UserId,
            task.ReminderSent,
            task.TaskTags.Select(tt => tt.Tag?.Name ?? tt.TagId.ToString()).ToList()
        );
    }
}
