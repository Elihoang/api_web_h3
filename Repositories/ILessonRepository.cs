using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetAllAsync();
    Task<Lesson?> GetByIdAsync(Guid id);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson?> UpdateAsync(Lesson lesson);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByCourseIdAndOrderAsync(Guid courseId, int orderNumber);
}