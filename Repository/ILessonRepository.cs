using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetAllLessons();
    Task<Lesson> GetLessonById(Guid id);
    Task CreateLesson(Lesson lesson);
    Task UpdateLesson(Lesson lesson);
    Task DeleteLesson(Guid id);
}