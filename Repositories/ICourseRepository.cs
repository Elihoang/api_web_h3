using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseByIdAsync(Guid id);
    Task AddCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Guid id);
    Task<string?> UploadImageAsync(Guid Id, IFormFile file);
    
    Task<IEnumerable<Course>> SearchCoursesAsync(string keyword, int page, int pageSize);
}
