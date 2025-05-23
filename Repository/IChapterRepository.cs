using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IChapterRepository
{
    Task<IEnumerable<Chapter>> GetAllChaptersAsync();
    Task<Chapter> GetChapterByIdAsync(string id);
    Task<IEnumerable<Chapter>> GetChaptersByCourseIdAsync(string courseId);
    Task AddChapterAsync(Chapter chapter);
    Task UpdateChapterAsync(Chapter chapter);
    Task DeleteChapterAsync(string id);
}