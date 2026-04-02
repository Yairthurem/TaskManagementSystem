namespace TaskManagementSystem.Api.Models;

public class RemindersLog
{
    public int Id { get; set; }
    
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    
    public DateTime ReminderDate { get; set; } = DateTime.UtcNow;
}
