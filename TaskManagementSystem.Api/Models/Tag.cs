using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Api.Models;

public class Tag
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
