using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetAllAsync();
    Task<Lesson> GetByIdAsync(Guid id);
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson> CreateAsync (Lesson lesson);
    Task<Lesson> UpdateAsync (Lesson lesson);
    Task<bool> DeleteAsync(Guid id);
    
}