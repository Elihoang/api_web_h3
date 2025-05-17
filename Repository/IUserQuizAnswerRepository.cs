using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IUserQuizAnswerRepository
{
    Task<UserQuizAnswer> AddAsync(UserQuizAnswer userAnswer);
    Task<UserQuizAnswer> GetByIdAsync(Guid id);
    Task<IEnumerable<UserQuizAnswer>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserQuizAnswer>> GetByQuizIdAsync(string quizId);
    Task<UserQuizAnswer> UpdateAsync(UserQuizAnswer userAnswer);
    Task<IEnumerable<UserQuizAnswer>> GetByLessonIdAsync(string lessonId, Guid userId);
    Task<bool> DeleteAsync(string id);
}