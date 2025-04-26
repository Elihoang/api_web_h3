namespace API_WebH3.DTO.UserNotification;

public class UserNotificationDto
{
    public Guid Id { get; set; }
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public bool IsRead { get; set; }
    public string SentAt { get; set; }
}