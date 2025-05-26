using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IFilterRepository
{
    Task<(IEnumerable<Course>, int)> FilterCoursesAsync(
        string? category, decimal? minPrice, decimal? maxPrice, double? minRating, int page, int pageSize);
}
