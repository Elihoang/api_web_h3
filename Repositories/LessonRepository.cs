using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson?> UpdateAsync(Lesson lesson)
    {
        var existingLesson = await _context.Lessons.FindAsync(lesson.Id);
        if (existingLesson == null)
        {
            return null;
        }

        _context.Entry(existingLesson).CurrentValues.SetValues(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
        {
            return false;
        }

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Include(l => l.Course)
            .OrderBy(l => l.OrderNumber)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByCourseIdAndOrderAsync(Guid courseId, int orderNumber)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId && l.OrderNumber == orderNumber)
            .Include(l => l.Course)
            .FirstOrDefaultAsync();
    }
}