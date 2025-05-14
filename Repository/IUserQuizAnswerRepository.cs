using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IUserQuizAnswerRepository
{
    Task<UserQuizAnswer> AddAsync(UserQuizAnswer userAnswer);
    Task<UserQuizAnswer> GetByIdAsync(Guid id);
    Task<IEnumerable<UserQuizAnswer>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserQuizAnswer>> GetByQuizIdAsync(string quizId);
}