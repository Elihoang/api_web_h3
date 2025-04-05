using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IPaymentRepository
{
    Task<List<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> UpdateAsync(Payment payment);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Payment>> GetByUserIdAsync(Guid userId);
    Task<List<Payment>> GetByCourseIdAsync(Guid courseId);
    Task<List<Payment>> GetByStatusAsync(string status);
} 