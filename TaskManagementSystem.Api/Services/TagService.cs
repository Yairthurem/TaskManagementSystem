using TaskManagementSystem.Api.DTOs;
using TaskManagementSystem.Api.Interfaces;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagResponseDto>> GetTagsAsync()
    {
        var tags = await _tagRepository.GetTagsAsync();
        return tags.Select(t => new TagResponseDto(t.Id, t.Name));
    }

    public async Task<TagResponseDto?> GetTagByIdAsync(int id)
    {
        var tag = await _tagRepository.GetTagByIdAsync(id);
        return tag == null ? null : new TagResponseDto(tag.Id, tag.Name);
    }

    public async Task<TagResponseDto> CreateTagAsync(TagCreateDto tagDto)
    {
        var tag = new Tag { Name = tagDto.Name };
        var created = await _tagRepository.AddTagAsync(tag);
        return new TagResponseDto(created.Id, created.Name);
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _tagRepository.GetTagByIdAsync(id);
        if (tag == null) return false;

        await _tagRepository.DeleteTagAsync(tag);
        return true;
    }
}
