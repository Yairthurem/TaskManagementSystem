using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Models;
using Xunit;

namespace TaskManagementSystem.Tests.Controllers;

public class UsersControllerTests
{
    // Best practice for testing EF Core directly bound into controllers: In-Memory DB
    private DbContextOptions<AppDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreateUser_HappyFlow_ShouldReturn201CreatedAndSave()
    {
        // Arrange
        using var context = new AppDbContext(GetInMemoryOptions());
        var controller = new UsersController(context);
        var dto = new UserCreateDto("John", "Doe", "john@test.com", "555-5555");

        // Act
        var result = await controller.CreateUser(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedDto = createdResult.Value.Should().BeOfType<UserDto>().Subject;
        
        returnedDto.FirstName.Should().Be("John");
        returnedDto.Email.Should().Be("john@test.com");
        
        // Confirm it securely saved inside the isolated InMemory SQL database
        context.Users.Should().HaveCount(1);
        context.Users.First().FirstName.Should().Be("John");
    }
    
    [Fact]
    public async Task GetUsers_HappyFlow_ShouldReturnMappedDtoList()
    {
        // Arrange
        using var context = new AppDbContext(GetInMemoryOptions());
        context.Users.Add(new User { FirstName = "Alice", LastName = "Smith", Email = "alice@test.com" });
        await context.SaveChangesAsync();
        
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var list = okResult.Value.Should().BeAssignableTo<IEnumerable<UserDto>>().Subject;
        
        list.Should().HaveCount(1);
        list.First().FirstName.Should().Be("Alice");
    }
}
