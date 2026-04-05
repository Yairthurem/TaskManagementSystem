using FluentAssertions;
using Moq;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;

namespace TaskManagementSystem.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _userService = new UserService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnMappedUserDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FirstName = "Alice", Email = "alice@test.com" },
            new User { Id = 2, FirstName = "Bob", Email = "bob@test.com" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "Alice" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        // Act
        var act = () => _userService.GetUserByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAddAndReturnMappedUser()
    {
        // Arrange
        var dto = new UserCreateDto("Alice", "Smith", "alice@test.com", "12345");
        var createdUser = new User { Id = 10, FirstName = "Alice", Email = "alice@test.com" };
        
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(dto);

        // Assert
        result.Id.Should().Be(10);
        result.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidId_ShouldUpdateAndReturnUser()
    {
        // Arrange
        var existingUser = new User { Id = 1, FirstName = "Old Name" };
        var updateDto = new UserUpdateDto("New Name", "New Last", "new@test.com", "54321");
        
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateUserAsync(1, updateDto);

        // Assert
        result.FirstName.Should().Be("New Name");
        _mockRepo.Verify(r => r.UpdateAsync(existingUser), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ShouldDelete()
    {
        // Arrange
        var user = new User { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        await _userService.DeleteUserAsync(1);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(user), Times.Once);
    }
}
