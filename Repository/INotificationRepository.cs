using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<Notification> GetByIdAsync(Guid id);
    Task AddAsync(Notification notification);
    Task DeleteAsync(Guid id);
    Task UpdateUserNotificationAsync(UserNotification userNotification);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
}
