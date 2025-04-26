using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.UserNotification;

public class CreateNotificationDto
{
    [Required]
    public string Type { get; set; }
    [Required]
    public string Content { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    [Required]
    public List<Guid> UserIds { get; set; }
}