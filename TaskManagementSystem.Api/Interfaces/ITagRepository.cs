using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag> AddTagAsync(Tag tag);
    Task DeleteTagAsync(Tag tag);
}
