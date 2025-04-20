using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ILessonApprovalRepository
{
    Task<IEnumerable<LessonApproval>> GetAllAsync();
    Task<LessonApproval> GetByIdAsync(Guid id);
    Task AddAsync(LessonApproval lessonApproval);
    Task UpdateAsync(LessonApproval lessonApproval);
    Task DeleteAsync(Guid id);
}