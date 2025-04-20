using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class ProgressRepository : IProgressRepository
{
    
    private readonly AppDbContext _context;

    public ProgressRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Progress>> GetAllProgressAsync()
    {
       return await _context.Progresses.ToListAsync();
    }

    public async Task<Progress> GetProgressByIdAsync(Guid id)
    {
        return await _context.Progresses.FindAsync(id);
    }

    public async Task AddProgressAsync(Progress progress)
    {
        await _context.Progresses.AddAsync(progress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProgressAsync(Progress progress)
    {
        _context.Progresses.Update(progress);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProgressAsync(Guid id)
    {
        var progress = await  _context.Progresses.FindAsync(id);
        if (progress != null)
        {
            _context.Progresses.Remove(progress);
            await _context.SaveChangesAsync();
        }
    }

    public async  Task<Progress> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        return await _context.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
    }
}