using API_WebH3.DTO.Notification;
using API_WebH3.DTO.UserNotification;
using API_WebH3.Models;
using API_WebH3.Repository;
using CreateNotificationDto = API_WebH3.DTO.Notification.CreateNotificationDto;

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
            RelatedEntityId = n.RelatedEntityId, // Đã là string?
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
    
    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Content = n.Content,
            RelatedEntityId = n.RelatedEntityId, // Đã là string?
            RelatedEntityType = n.RelatedEntityType,
            CreatedAt = n.CreatedAt,
            UserNotifications = n.UserNotifications?.Where(un => un.UserId == userId).Select(un => new UserNotificationDto
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
            RelatedEntityId = notification.RelatedEntityId, // Đã là string?
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
            throw new ArgumentException("Loại thông báo không hợp lệ. Phải là 'LessonApproval', 'NewMessage', hoặc 'CourseEnrollment'.");
        }

        if ((createNotificationDto.RelatedEntityId != null) != (createNotificationDto.RelatedEntityType != null))
        {
            throw new ArgumentException("RelatedEntityId và RelatedEntityType phải được cung cấp cùng nhau hoặc đều rỗng.");
        }

        // Không cần parse RelatedEntityId nữa vì đã là string?
        string? relatedEntityId = createNotificationDto.RelatedEntityId;

        // Nếu là thông báo toàn hệ thống, lấy tất cả người dùng
        if (createNotificationDto.IsSystemWide)
        {
            var allUsers = await _userRepository.GetAllAsync();
            createNotificationDto.UserIds = allUsers.Select(u => u.Id).ToList();
        }
        else if (!createNotificationDto.UserIds.Any())
        {
            throw new ArgumentException("Phải chỉ định ít nhất một người dùng để nhận thông báo.");
        }

        // Kiểm tra tính hợp lệ của userIds
        foreach (var userId in createNotificationDto.UserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Không tìm thấy người dùng với ID {userId}.");
            }
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Type = createNotificationDto.Type,
            Content = createNotificationDto.Content,
            RelatedEntityId = relatedEntityId, // Gán trực tiếp string?
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
            RelatedEntityId = notification.RelatedEntityId, // Đã là string?
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
    
    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
        {
            throw new ArgumentException("Không tìm thấy thông báo");
        }

        var userNotification = notification.UserNotifications
            .FirstOrDefault(un => un.UserId == userId);
        if (userNotification == null)
        {
            throw new ArgumentException("Người dùng không có thông báo này");
        }

        userNotification.IsRead = true;
        await _notificationRepository.UpdateUserNotificationAsync(userNotification);
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