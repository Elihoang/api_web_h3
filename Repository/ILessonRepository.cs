using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetAllLessons();
    Task<Lesson> GetLessonById(string id);
    Task<IEnumerable<Lesson>> GetLessonsByChapterIdAsync(Guid chapterId);
    Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(string courseId);
    Task CreateLesson(Lesson lesson);
    Task UpdateLesson(Lesson lesson);
    Task DeleteLesson(string id);
}