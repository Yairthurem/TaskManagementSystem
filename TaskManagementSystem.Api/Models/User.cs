using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Api.Models;

public class User
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Phone { get; set; }
}
