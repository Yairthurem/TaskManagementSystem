using FluentAssertions;
using Moq;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;

namespace TaskManagementSystem.Tests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _mockRepo;
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _mockRepo = new Mock<ITagRepository>();
        _tagService = new TagService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetTagsAsync_ShouldReturnMappedTagDtos()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new Tag { Id = 1, Name = "Critical" },
            new Tag { Id = 2, Name = "Bug" }
        };
        _mockRepo.Setup(r => r.GetTagsAsync()).ReturnsAsync(tags);

        // Act
        var result = await _tagService.GetTagsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Critical");
    }

    [Fact]
    public async Task GetTagByIdAsync_WithValidId_ShouldReturnTag()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Critical" };
        _mockRepo.Setup(r => r.GetTagByIdAsync(1)).ReturnsAsync(tag);

        // Act
        var result = await _tagService.GetTagByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Critical");
    }

    [Fact]
    public async Task CreateTagAsync_ShouldAddAndReturnMappedTag()
    {
        // Arrange
        var dto = new TagCreateDto("Urgent");
        var createdTag = new Tag { Id = 10, Name = "Urgent" };
        
        _mockRepo.Setup(r => r.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(createdTag);

        // Act
        var result = await _tagService.CreateTagAsync(dto);

        // Assert
        result.Id.Should().Be(10);
        result.Name.Should().Be("Urgent");
        _mockRepo.Verify(r => r.AddTagAsync(It.Is<Tag>(t => t.Name == "Urgent")), Times.Once);
    }

    [Fact]
    public async Task DeleteTagAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var tag = new Tag { Id = 1 };
        _mockRepo.Setup(r => r.GetTagByIdAsync(1)).ReturnsAsync(tag);

        // Act
        var result = await _tagService.DeleteTagAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(r => r.DeleteTagAsync(tag), Times.Once);
    }
}
