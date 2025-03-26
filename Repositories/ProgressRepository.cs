using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class ProgressRepository:IProgressRepository
{
    private readonly AppDbContext _context;

    public ProgressRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<Progress>> GetAllAsync()
    {
        return await _context.Progresses.ToListAsync();
    }

    public async Task<Progress> GetByIdAsync(Guid id)
    {
        return await _context.Progresses
            .Include(p=>p.User)
            .Include(p=>p.Lesson)
            .FirstOrDefaultAsync(p=>p.Id==id);
        
    }

    public async Task<Progress> CreateAsync(Progress progress)
    {
        _context.Progresses.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<Progress> UpdateAsync(Progress progress)
    {
        _context.Progresses.Update(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var progress = await GetByIdAsync(id);
        if (progress == null)
        {
            return false;
        }

        _context.Progresses.Remove(progress);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsByUserIdAndLessonIdAsync(Guid userId, Guid lessonId)
    {
        return await _context.Progresses
            .AnyAsync(p => p.UserId == userId && p.LessonId == lessonId);
    }
}
