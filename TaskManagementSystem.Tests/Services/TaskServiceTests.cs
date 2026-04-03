using FluentAssertions;
using Moq;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;
using Xunit;

namespace TaskManagementSystem.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_HappyFlow_ShouldReturnMappedTask()
    {
        // 1. Arrange
        var request = new TaskCreateDto("New Feature", "Description", null, TaskPriority.High, 1, new List<int> { 5 });
        
        // Mocking validation checks to pass
        _mockRepository.Setup(r => r.UserExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.TagsExistAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(true);
        
        var savedTask = new TaskEntity 
        { 
            Id = 10, Title = "New Feature", Priority = TaskPriority.High, UserId = 1 
        };
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<TaskEntity>())).ReturnsAsync(savedTask);
        // Mock returning the associated tag cleanly
        _mockRepository.Setup(r => r.GetTagsByTaskAsync(10)).ReturnsAsync(new List<Tag> { new Tag { Name = "Urgent" }});

        // 2. Act
        var result = await _taskService.CreateTaskAsync(request);

        // 3. Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Title.Should().Be("New Feature");
        result.Tags.Should().Contain("Urgent");
        
        // Ensure Database constraints were verified before saving
        _mockRepository.Verify(r => r.UserExistsAsync(1), Times.Once); 
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<TaskEntity>()), Times.Once);
    }
}
