using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class EnrollmentRepository: IEnrollmentRepository
{
    
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments.ToListAsync();
    }
    public async Task<Enrollment> GetByIdAsync(int id)
    {
        return await _context.Enrollments.FindAsync(id);
    }

    public async Task<Enrollment> GetByUserAndCourseAsync(Guid userId, string courseId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<IEnumerable<Enrollment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Enrollments
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId)
    {
        return await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}