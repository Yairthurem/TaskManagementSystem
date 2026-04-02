using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.DTOs;

public record TaskCreateDto(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority,
    int UserId,
    List<int>? TagIds
);

public record TaskUpdateDto(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority,
    int UserId,
    List<int>? TagIds
);

public record TaskResponseDto(
    int Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime? DueDate,
    TaskPriority Priority,
    int UserId,
    bool ReminderSent,
    List<string> Tags
);
