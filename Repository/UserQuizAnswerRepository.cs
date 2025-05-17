using API_WebH3.Data;
using API_WebH3.Models;
using API_WebH3.Repository;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class UserQuizAnswerRepository : IUserQuizAnswerRepository
{
    private readonly AppDbContext _context;

    public UserQuizAnswerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserQuizAnswer> AddAsync(UserQuizAnswer userAnswer)
    {
        _context.UserQuizAnswers.Add(userAnswer);
        await _context.SaveChangesAsync();
        return userAnswer;
    }

    public async Task<UserQuizAnswer> GetByIdAsync(Guid id)
    {
        return await _context.UserQuizAnswers.FindAsync(id);
    }

    public async Task<IEnumerable<UserQuizAnswer>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserQuizAnswers
            .Where(uqa => uqa.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserQuizAnswer>> GetByQuizIdAsync(string quizId)
    {
        return await _context.UserQuizAnswers
            .Where(uqa => uqa.QuizId == quizId)
            .ToListAsync();
    }
    public async Task<IEnumerable<UserQuizAnswer>> GetByLessonIdAsync(string lessonId, Guid userId)
    {
        return await _context.UserQuizAnswers
            .Where(a => a.Quiz.LessonId == lessonId && a.UserId == userId)
            .ToListAsync();
    }
    public async Task<UserQuizAnswer> UpdateAsync(UserQuizAnswer userAnswer)
    {
        _context.UserQuizAnswers.Update(userAnswer);
        await _context.SaveChangesAsync();
        return userAnswer;
    }
    public async Task<bool> DeleteAsync(string id)
    {
        var answer = await _context.UserQuizAnswers.FindAsync(id);
        if (answer == null) return false;

        _context.UserQuizAnswers.Remove(answer);
        await _context.SaveChangesAsync();
        return true;
    }
}