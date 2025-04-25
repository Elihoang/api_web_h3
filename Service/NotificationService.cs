using API_WebH3.DTO.Notification;
using API_WebH3.DTO.UserNotification;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository; 
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly IUserRepository _userRepository;
    
    public NotificationService(INotificationRepository notificationRepository, IUserNotificationRepository userNotificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userNotificationRepository = userNotificationRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Content = n.Content,
            RelatedEntityId = n.RelatedEntityId,
            RelatedEntityType = n.RelatedEntityType,
            CreatedAt = n.CreatedAt,
            UserNotifications = n.UserNotifications?.Select(un => new UserNotificationDto
            {
                Id = un.Id,
                NotificationId = un.NotificationId,
                UserId = un.UserId,
                IsRead = un.IsRead,
                SentAt = un.SentAt
            }).ToList() ?? new List<UserNotificationDto>()
        });
    }
    public async Task<NotificationDto> GetByIdAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
        {
            return null;
        }
        return new NotificationDto
        {
            Id = notification.Id,
            Type = notification.Type,
            Content = notification.Content,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType,
            CreatedAt = notification.CreatedAt,
            UserNotifications = notification.UserNotifications?.Select(un => new UserNotificationDto
            {
                Id = un.Id,
                NotificationId = un.NotificationId,
                UserId = un.UserId,
                IsRead = un.IsRead,
                SentAt = un.SentAt
            }).ToList() ?? new List<UserNotificationDto>()
        };
    }
    
    public async Task<NotificationDto> CreateAsync(CreateNotificationDto createNotificationDto)
    { 
        var validTypes = new[] { "LessonApproval", "NewMessage", "CourseEnrollment" };
        if (!validTypes.Contains(createNotificationDto.Type))
        {
            throw new ArgumentException("Invalid notification type. Must be 'LessonApproval', 'NewMessage', or 'CourseEnrollment'.");
        }

        if (createNotificationDto.RelatedEntityId.HasValue != (createNotificationDto.RelatedEntityType != null))
        {
            throw new ArgumentException("RelatedEntityId and RelatedEntityType must both be provided or both be null.");
        }

        if (!createNotificationDto.UserIds.Any())
        {
            throw new ArgumentException("At least one user must be specified to receive the notification.");
        }

        foreach (var userId in createNotificationDto.UserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found.");
        }
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Type = createNotificationDto.Type,
            Content = createNotificationDto.Content,
            RelatedEntityId = createNotificationDto.RelatedEntityId,
            RelatedEntityType = createNotificationDto.RelatedEntityType,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            UserNotifications = createNotificationDto.UserIds.Select(userId => new UserNotification
            {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationId = Guid.Empty,
            IsRead = false,
            SentAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        }).ToList()
        };

        foreach (var userNotification in notification.UserNotifications)
        {
            userNotification.NotificationId = notification.Id;
        }

        await _notificationRepository.AddAsync(notification);

        return new NotificationDto
        {
            Id = notification.Id,
            Type = notification.Type,
            Content = notification.Content,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType,
            CreatedAt = notification.CreatedAt,
            UserNotifications = notification.UserNotifications.Select(un => new UserNotificationDto
            {
            Id = un.Id,
            NotificationId = un.NotificationId,
            UserId = un.UserId,
            IsRead = un.IsRead,
            SentAt = un.SentAt
        }).ToList()
        };
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
        {
            return false;
        }

        await _notificationRepository.DeleteAsync(id);
        return true;
    }
}