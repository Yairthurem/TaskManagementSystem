using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Api.Models;

public enum TaskPriority { Low, Medium, High }

public class TaskEntity
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool ReminderSent { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<RemindersLog> RemindersLogs { get; set; } = new List<RemindersLog>();
}
