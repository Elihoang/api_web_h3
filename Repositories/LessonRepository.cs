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
        
        
    public async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        return await _context.Lessons.ToListAsync();
    }

    public async Task<Lesson> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l=>l.Id == id);  
    }
    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons.Where(c => c.CourseId == courseId).ToListAsync();
    }

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
       _context.Lessons.Update(lesson);
       await _context.SaveChangesAsync();
       return lesson;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var lesson =await GetByIdAsync(id);
        if (lesson == null) return false;
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }
}