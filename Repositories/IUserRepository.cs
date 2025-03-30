using API_WebH3.Models;

namespace API_WebH3.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(Guid userId);
    }

}
