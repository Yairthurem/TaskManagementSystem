using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetTasksAsync();
    Task<TaskResponseDto?> GetTaskAsync(int id);
    Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto taskDto);
    Task<bool> UpdateTaskAsync(int id, TaskUpdateDto taskDto);
    Task<bool> DeleteTaskAsync(int id);
}
