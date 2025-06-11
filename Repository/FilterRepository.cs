using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class FilterRepository : IFilterRepository
{
    private readonly AppDbContext _context;

    public FilterRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Course>, int)> FilterCoursesAsync(
        string? category, decimal? minPrice, decimal? maxPrice, double? minRating, int page, int pageSize)
    {
        var find = _context.Courses
            .Include(c => c.Category)
            .Include(c => c.User)
            .GroupJoin(_context.Reviews,
                course => course.Id,
                review => review.CourseId,
                (course, reviews) => new
                {
                    Course = course,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
                })
            .Where(c=>c.Course.Activate=="Active")
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            find = find.Where(c => c.Course.Category != null &&
                                   c.Course.Category.Name != null &&
                                   c.Course.Category.Name.ToLower().Contains(category.ToLower()) || c.Course.Category.Id == category);
   
        }

        if (minPrice.HasValue)
        {
            find = find.Where(c => c.Course.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            find = find.Where(c => c.Course.Price <= maxPrice.Value);
        }

        if (minRating.HasValue)
        {
            find = find.Where(c => c.AverageRating >= minRating.Value);
        }

        var total = await find.CountAsync();

        var courses = await find
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.Course)
            .ToListAsync();

        return (courses, total);
    }
}