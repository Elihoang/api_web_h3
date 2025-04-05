using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IProgressRepository
{
    Task<List<Progress>> GetAllAsync();
    Task<Progress?> GetByIdAsync(Guid id);
    Task<Progress> CreateAsync(Progress progress);
    Task<Progress?> UpdateAsync(Progress progress);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Progress>> GetByUserIdAsync(Guid userId);
    Task<List<Progress>> GetByLessonIdAsync(Guid lessonId);
    Task<Progress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
    Task<List<Progress>> GetByCourseIdAsync(Guid courseId, Guid userId);
}