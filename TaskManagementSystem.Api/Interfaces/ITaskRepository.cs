using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetTasksAsync();
    Task<TaskEntity?> GetTaskByIdAsync(int id);
    Task<TaskEntity> AddTaskAsync(TaskEntity task);
    Task UpdateTaskAsync(TaskEntity task);
    Task DeleteTaskAsync(TaskEntity task);
}
