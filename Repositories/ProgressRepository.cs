using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly AppDbContext _context;

    public ProgressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Progress>> GetAllAsync()
    {
        return await _context.Progresses
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .ToListAsync();
    }

    public async Task<Progress?> GetByIdAsync(Guid id)
    {
        return await _context.Progresses
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Progress> CreateAsync(Progress progress)
    {
        _context.Progresses.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<Progress?> UpdateAsync(Progress progress)
    {
        var existingProgress = await _context.Progresses.FindAsync(progress.Id);
        if (existingProgress == null)
        {
            return null;
        }

        _context.Entry(existingProgress).CurrentValues.SetValues(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var progress = await _context.Progresses.FindAsync(id);
        if (progress == null)
        {
            return false;
        }

        _context.Progresses.Remove(progress);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Progress>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Progresses
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .OrderByDescending(p => p.LastUpdate)
            .ToListAsync();
    }

    public async Task<List<Progress>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _context.Progresses
            .Where(p => p.LessonId == lessonId)
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .OrderByDescending(p => p.LastUpdate)
            .ToListAsync();
    }

    public async Task<Progress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        return await _context.Progresses
            .Where(p => p.UserId == userId && p.LessonId == lessonId)
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Progress>> GetByCourseIdAsync(Guid courseId, Guid userId)
    {
        return await _context.Progresses
            .Where(p => p.UserId == userId && p.Lesson.CourseId == courseId)
            .Include(p => p.User)
            .Include(p => p.Lesson)
            .OrderBy(p => p.Lesson.OrderNumber)
            .ToListAsync();
    }

    public async Task<bool> ExistsByUserIdAndLessonIdAsync(Guid userId, Guid lessonId)
    {
        return await _context.Progresses
            .AnyAsync(p => p.UserId == userId && p.LessonId == lessonId);
    }
}
