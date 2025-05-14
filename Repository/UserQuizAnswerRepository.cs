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
}