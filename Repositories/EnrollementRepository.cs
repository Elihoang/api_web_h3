using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class EnrollementRepository : IEnrollementRepository
{
    private readonly AppDbContext _context;

    public EnrollementRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments.ToListAsync();
    }

    public async Task<Enrollment> GetByIdAsync(int id)
    {
        return await _context.Enrollments
            .Include(u => u.User)
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await GetByIdAsync(id);
        if (enrollment == null)
        {
            return false;
        }
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<Enrollment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Enrollments
            .Where(e => e.UserId == userId) // L?c theo userId
            .Include(u => u.User) // Bao g?m thông tin User
            .Include(c => c.Course) // Bao g?m thông tin Course
            .ToListAsync();
    }
}