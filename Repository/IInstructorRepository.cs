using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IInstructorRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
}