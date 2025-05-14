using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllAsync();
    Task<Quiz> GetByIdAsync(string id);
    Task<IEnumerable<Quiz>> GetByLessonIdAsync(string lessonId);
    Task<Quiz> AddAsync(Quiz quiz);
    Task<Quiz> UpdateAsync(Quiz quiz);
    Task<bool> DeleteAsync(string id);
}