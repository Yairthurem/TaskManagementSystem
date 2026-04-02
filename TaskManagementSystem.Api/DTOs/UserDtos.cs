namespace TaskManagementSystem.Api.DTOs;

public record UserDto(int Id, string FirstName, string LastName, string Email, string? Phone);
public record UserCreateDto(string FirstName, string LastName, string Email, string? Phone);
public record UserUpdateDto(string FirstName, string LastName, string Email, string? Phone);
