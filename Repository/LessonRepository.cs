using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Lesson>> GetAllLessons()
    {
        return await _context.Lessons.ToListAsync();
    }

    public async Task<Lesson> GetLessonById(string id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByChapterIdAsync(Guid chapterId)
    {
        return await _context.Lessons
            .Where(l => l.ChapterId == chapterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(string courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync();
    }

    public async Task CreateLesson(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLesson(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLesson(string id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}