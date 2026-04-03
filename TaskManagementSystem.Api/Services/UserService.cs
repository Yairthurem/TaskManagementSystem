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

    public async Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;

        await _userRepository.UpdateAsync(user);

        return new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Phone);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

        await _userRepository.DeleteAsync(user);
    }
}
