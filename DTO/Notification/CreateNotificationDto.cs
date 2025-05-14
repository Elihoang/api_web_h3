namespace API_WebH3.DTO.Notification;

public class CreateNotificationDto
{
    public string Type { get; set; }
    public string Content { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public List<Guid> UserIds { get; set; } = new List<Guid>();
    public bool IsSystemWide { get; set; } // Thêm trường mới
}