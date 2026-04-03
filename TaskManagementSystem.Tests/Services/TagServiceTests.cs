using FluentAssertions;
using Moq;
using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Services;
using Xunit;

namespace TaskManagementSystem.Tests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _mockRepository;
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _mockRepository = new Mock<ITagRepository>();
        _tagService = new TagService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateTagAsync_HappyFlow_ShouldReturnCreatedTag()
    {
        // Arrange
        var dto = new TagCreateDto("Urgent");
        var insertedTag = new Tag { Id = 1, Name = "Urgent" };
        
        _mockRepository.Setup(r => r.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(insertedTag);

        // Act
        var result = await _tagService.CreateTagAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Urgent");
        
        // Confirm repository method was called exactly once with the correctly mapped DTO data
        _mockRepository.Verify(r => r.AddTagAsync(It.Is<Tag>(t => t.Name == "Urgent")), Times.Once);
    }

    [Fact]
    public async Task GetTagsAsync_HappyFlow_ShouldReturnListOfTags()
    {
        // Arrange
        var tagsList = new List<Tag>
        {
            new Tag { Id = 1, Name = "Bug" },
            new Tag { Id = 2, Name = "Feature" }
        };
        
        _mockRepository.Setup(r => r.GetTagsAsync()).ReturnsAsync(tagsList);

        // Act
        var result = await _tagService.GetTagsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Bug");
        _mockRepository.Verify(r => r.GetTagsAsync(), Times.Once);
    }
}
