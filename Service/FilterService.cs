using API_WebH3.DTO.Course;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class FilterService
{
    private readonly IFilterRepository _filterRepository;

    public FilterService(IFilterRepository filterRepository)
    {
        _filterRepository = filterRepository;
    }

    public async Task<(IEnumerable<CourseDto>, int, int)> FilterCoursesAsync(
        string? category, decimal? minPrice, decimal? maxPrice, double? minRating, int page, int pageSize)
    {
        try
        {
            var (courses, total) = await _filterRepository.FilterCoursesAsync(
                category, minPrice, maxPrice, minRating, page, pageSize);
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var courseDtos = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                UrlImage = c.UrlImage,
                InstructorId = c.InstructorId,
                CategoryId = c.CategoryId,
                CreatedAt = c.CreatedAt,
                Contents = c.Contents
            }).ToList();

            return (courseDtos, total, totalPages);
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Lỗi lọc khóa học: {ex.Message}");
            throw;
        }
    }
}