using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IUserNotificationRepository
{
    Task<IEnumerable<UserNotification>> GetAllAsync();
    Task<UserNotification> GetByIdAsync(Guid id);
    Task<UserNotification> GetByUserAndNotificationAsync(Guid userId, Guid notificationId);
    Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserNotification userNotification);
    Task UpdateAsync(UserNotification userNotification);
    Task DeleteAsync(Guid id);
}