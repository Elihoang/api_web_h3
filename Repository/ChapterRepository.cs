using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class ChapterRepository : IChapterRepository
{
    private readonly AppDbContext _context;

    public ChapterRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Chapter>> GetAllChaptersAsync()
    {
        return await _context.Chapters.ToListAsync();
    }

    public async Task<Chapter> GetChapterByIdAsync(string id)
    {
        return await _context.Chapters.FindAsync(id);
    }

    public async Task<IEnumerable<Chapter>> GetChaptersByCourseIdAsync(string courseId)
    {
        return await _context.Chapters
            .Where(c => c.CourseId == courseId)
            .ToListAsync();
    }

    public async Task AddChapterAsync(Chapter chapter)
    {
        await _context.Chapters.AddAsync(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateChapterAsync(Chapter chapter)
    {
        _context.Chapters.Update(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChapterAsync(string id)
    {
        var chapter = await _context.Chapters.FindAsync(id);
        if (chapter != null)
        {
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }
    }
}