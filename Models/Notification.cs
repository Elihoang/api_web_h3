using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_WebH3.Models;

namespace API_WebH3.Models;

// Notification model to represent a notification event
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public required string Type { get; set; } // e.g., LessonApproval, NewMessage, CourseEnrollment

    [Required]
    public required string Content { get; set; } // Notification message (e.g., "Your lesson was approved")

    public Guid? RelatedEntityId { get; set; } // Optional ID of related entity (e.g., LessonId, MessageId)

    public string? RelatedEntityType { get; set; } // Type of related entity (e.g., Lesson, Message, Course)

    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}