using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<Course> GetByIdAsync(string id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Courses.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetCoursesByUserIdAsync(string userId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == Guid.Parse(userId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesByCategoryIdAsync(string categoryId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == Guid.Parse(categoryId))
            .ToListAsync();
    }
    public async Task<IEnumerable<Course>> SearchCoursesAsync(string keyword, int page, int pageSize)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(keyword) ||
                                     (c.Description != null && c.Description.ToLower().Contains(keyword)));
        }

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<Course>> GetAllActiveCoursesAsync()
    {
        return await _context.Courses
            .Where(c => c.Activate == "Active")
            .ToListAsync();
    }
}