using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IProgressRepository
{
    Task<IEnumerable<Progress>> GetAllProgressAsync();
    Task<Progress> GetProgressByIdAsync(Guid id);
    Task AddProgressAsync(Progress progress);
    Task UpdateProgressAsync(Progress progress);
    Task DeleteProgressAsync(Guid id);
    Task<Progress> GetByUserAndLessonAsync(Guid userId, string lessonId);
}