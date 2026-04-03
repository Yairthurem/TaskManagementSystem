using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> CreateUserAsync(UserCreateDto dto);
}
