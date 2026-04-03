using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using Xunit;

namespace TaskManagementSystem.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        // Now accurately matching the Mock pattern established in Task/Tags!
        _mockUserService = new Mock<IUserService>();
        _controller = new UsersController(_mockUserService.Object);
    }

    [Fact]
    public async Task CreateUser_HappyFlow_ShouldReturn201Created()
    {
        // 1. Arrange
        var requestDto = new UserCreateDto("John", "Doe", "john@test.com", "555-5555");
        var newlyCreatedUser = new UserDto(1, "John", "Doe", "john@test.com", "555-5555");
        
        _mockUserService.Setup(s => s.CreateUserAsync(requestDto)).ReturnsAsync(newlyCreatedUser);

        // 2. Act
        var result = await _controller.CreateUser(requestDto);

        // 3. Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedDto = createdResult.Value.Should().BeOfType<UserDto>().Subject;
        
        returnedDto.FirstName.Should().Be("John");
        returnedDto.Email.Should().Be("john@test.com");
        
        _mockUserService.Verify(s => s.CreateUserAsync(requestDto), Times.Once);
    }
}
