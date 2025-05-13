using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course> GetByIdAsync(string id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Course>> GetCoursesByUserIdAsync(string userId);
    Task<IEnumerable<Course>> GetCoursesByCategoryIdAsync(string categoryId);
}