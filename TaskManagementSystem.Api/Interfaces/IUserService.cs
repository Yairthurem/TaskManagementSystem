using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(UserCreateDto dto);
    Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto);
    Task DeleteUserAsync(int id);
}
