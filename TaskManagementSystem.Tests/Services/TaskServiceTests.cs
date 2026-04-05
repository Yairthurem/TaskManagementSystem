using FluentAssertions;
using Moq;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;

namespace TaskManagementSystem.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepo;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnMappedResponseDtos()
    {
        // Arrange
        var tasks = new List<TaskEntity>
        {
            new TaskEntity { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow, Priority = TaskPriority.Medium },
            new TaskEntity { Id = 2, Title = "Task 2", CreatedAt = DateTime.UtcNow, Priority = TaskPriority.High }
        };
        _mockRepo.Setup(r => r.GetTasksAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetTasksAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Task 1");
        _mockRepo.Verify(r => r.GetTasksAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTaskAsync_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var task = new TaskEntity { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetTaskAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var result = await _taskService.GetTaskAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAddAndReturnMappedTask()
    {
        // Arrange
        var dto = new TaskCreateDto("New Title", "Description", null, TaskPriority.High, 1, new List<int> { 1 });
        var createdTask = new TaskEntity { Id = 10, Title = "New Title", UserId = 1, CreatedAt = DateTime.UtcNow };
        
        _mockRepo.Setup(r => r.AddTaskAsync(It.IsAny<TaskEntity>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTaskAsync(dto);

        // Assert
        result.Id.Should().Be(10);
        result.Title.Should().Be("New Title");
        _mockRepo.Verify(r => r.AddTaskAsync(It.Is<TaskEntity>(t => t.Title == "New Title")), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithValidId_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var existingTask = new TaskEntity { Id = 1, Title = "Old Title", DueDate = DateTime.UtcNow.AddDays(-1), ReminderSent = true };
        var updateDto = new TaskUpdateDto("New Title", "New Desc", DateTime.UtcNow.AddDays(1), TaskPriority.Medium, 1, new List<int>());
        
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(1, updateDto);

        // Assert
        result.Should().BeTrue();
        existingTask.Title.Should().Be("New Title");
        existingTask.ReminderSent.Should().BeFalse(); // Reset logic check
        _mockRepo.Verify(r => r.UpdateTaskAsync(existingTask), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var task = new TaskEntity { Id = 1 };
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _taskService.DeleteTaskAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(r => r.DeleteTaskAsync(task), Times.Once);
    }
}
