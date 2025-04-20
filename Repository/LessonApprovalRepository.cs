using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class LessonApprovalRepository : ILessonApprovalRepository
{
    private readonly AppDbContext _context;

    public LessonApprovalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LessonApproval>> GetAllAsync()
    {
        return await _context.LessonApprovals.ToListAsync();
    }

    public async Task<LessonApproval> GetByIdAsync(Guid id)
    {
        return await _context.LessonApprovals.FindAsync(id);
    }

    public async Task AddAsync(LessonApproval lessonApproval)
    {
        await _context.LessonApprovals.AddAsync(lessonApproval);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LessonApproval lessonApproval)
    {
        _context.LessonApprovals.Update(lessonApproval);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var lessonApproval =  await _context.LessonApprovals.FindAsync(id);
        if (lessonApproval != null)
        {
            _context.LessonApprovals.Remove(lessonApproval);
            await _context.SaveChangesAsync();
        }
        
    }
}