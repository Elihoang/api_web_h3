using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetAllAsync();
    Task<Chat> GetByIdAsync(Guid id);
    Task<Chat> GetByUsersAsync(Guid user1Id, Guid user2Id);
    Task<IEnumerable<Chat>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Chat chat);
    Task DeleteAsync(Guid id);
}