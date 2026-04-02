namespace TaskManagementSystem.Api.Models;

public class TaskTag
{
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
