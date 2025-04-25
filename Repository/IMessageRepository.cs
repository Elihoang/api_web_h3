using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAllAsync();
    Task<Message> GetByIdAsync(Guid id);
    Task<IEnumerable<Message>> GetByChatIdAsync(Guid chatId);
    Task AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task DeleteAsync(Guid id);
}