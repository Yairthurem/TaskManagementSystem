using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksAsync()
    {
        return await _context.Tasks
                             .Include(t => t.TaskTags)
                                .ThenInclude(tt => tt.Tag)
                             .OrderBy(t => t.DueDate)
                             .ToListAsync();
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks
                             .Include(t => t.TaskTags)
                                .ThenInclude(tt => tt.Tag)
                             .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskEntity> AddTaskAsync(TaskEntity task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(TaskEntity task)
    {
        task.IsDeleted = true;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }
}
