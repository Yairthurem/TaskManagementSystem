using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        // Domain modeling mapping logic
        return users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email, u.Phone));
    }

    public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
    {
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone
        };

        var createdUser = await _userRepository.AddAsync(user);
        
        return new UserDto(
            createdUser.Id,
            createdUser.FirstName,
            createdUser.LastName,
            createdUser.Email,
            createdUser.Phone
        );
    }
}
