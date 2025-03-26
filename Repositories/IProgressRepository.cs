using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IProgressRepository
{
    Task<List<Progress>> GetAllAsync();
    Task<Progress> GetByIdAsync(Guid id);
    Task<Progress> CreateAsync(Progress progress);
    Task<Progress> UpdateAsync(Progress progress);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByUserIdAndLessonIdAsync(Guid userId, Guid lessonId);
}