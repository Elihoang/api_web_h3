using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllAsync();
    Task<Enrollment> GetByIdAsync(int id);
    Task<Enrollment> GetByUserAndCourseAsync(Guid userId, string courseId);
    Task<IEnumerable<Enrollment>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId);
    Task AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task  DeleteAsync(int id);
    Task DeleteEnrollmentAsync(Guid userId, string courseId);
    Task<Enrollment> GetEnrollmentAsync(Guid userId, string courseId);
    Task UpdateEnrollmentStatusAsync(Guid userId, string courseId, string status);
}