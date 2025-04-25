using API_WebH3.DTO.UserNotification;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class UserNotificationService
{
    private readonly IUserNotificationRepository _userNotificationRepository;
    public UserNotificationService(IUserNotificationRepository userNotificationRepository)
    {
        _userNotificationRepository = userNotificationRepository;
    }
    public async Task<IEnumerable<UserNotificationDto>> GetAllAsync()
    {
        var userNotifications = await _userNotificationRepository.GetAllAsync();
        return userNotifications.Select(un => new UserNotificationDto
        {
            Id = un.Id,
            NotificationId = un.NotificationId,
            UserId = un.UserId,
            IsRead = un.IsRead,
            SentAt = un.SentAt
        });
    }
    public async Task<UserNotificationDto> GetByIdAsync(Guid id)
    {
        var userNotification = await _userNotificationRepository.GetByIdAsync(id);
        if (userNotification == null)
        {
            return null;
        }
        return new UserNotificationDto
        {
            Id = userNotification.Id,
            NotificationId = userNotification.NotificationId,
            UserId = userNotification.UserId,
            IsRead = userNotification.IsRead,
            SentAt = userNotification.SentAt
        };
    }
    public async Task<IEnumerable<UserNotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var userNotifications = await _userNotificationRepository.GetByUserIdAsync(userId);
        return userNotifications.Select(un => new UserNotificationDto
        {
            Id = un.Id,
            NotificationId = un.NotificationId,
            UserId = un.UserId,
            IsRead = un.IsRead,
            SentAt = un.SentAt
        });
    }
    public async Task<UserNotificationDto> UpdateAsync(Guid id, UpdateUserNotificationDto updateUserNotificationDto)
    {
        var userNotification = await _userNotificationRepository.GetByIdAsync(id);
        if (userNotification == null)
        {
            return null;
        }

        userNotification.IsRead = updateUserNotificationDto.IsRead;

        await _userNotificationRepository.UpdateAsync(userNotification);

        return new UserNotificationDto
        {
            Id = userNotification.Id,
            NotificationId = userNotification.NotificationId,
            UserId = userNotification.UserId,
            IsRead = userNotification.IsRead,
            SentAt = userNotification.SentAt
        };
    }public async Task<bool> DeleteAsync(Guid id)
    {
        var userNotification = await _userNotificationRepository.GetByIdAsync(id);
        if (userNotification==null)
        {
            return false;
        }

        await _userNotificationRepository.DeleteAsync(id);
        return true;
    }
}