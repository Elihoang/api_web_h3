using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IStudentRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(string id);
    Task<User> GetByEmailAsync(string email);
}