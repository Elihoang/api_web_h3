using API_WebH3.DTO.UserNotification;

namespace API_WebH3.DTO.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string CreatedAt { get; set; }
    public List<UserNotificationDto> UserNotifications { get; set; } = new List<UserNotificationDto>();
}