using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IEnrollementRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<Enrollment> GetByIdAsync(int id);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<Enrollment> UpdateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
}