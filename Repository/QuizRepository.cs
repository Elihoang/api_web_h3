using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;

    public QuizRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes.Include(q => q.Lesson).ToListAsync();
    }

    public async Task<Quiz> GetByIdAsync(string id)
    {
        return await _context.Quizzes.Include(q => q.Lesson)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Quiz>> GetByLessonIdAsync(string lessonId)
    {
        return await _context.Quizzes
            .Where(q => q.LessonId == lessonId)
            .Include(q => q.Lesson)
            .ToListAsync();
    }

    public async Task<Quiz> AddAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task<Quiz> UpdateAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return true;
    }
}