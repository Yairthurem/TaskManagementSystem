using TaskManagementSystem.Api.DTOs;

namespace TaskManagementSystem.Api.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagResponseDto>> GetTagsAsync();
    Task<TagResponseDto?> GetTagByIdAsync(int id);
    Task<TagResponseDto> CreateTagAsync(TagCreateDto tagDto);
    Task<bool> DeleteTagAsync(int id);
}
