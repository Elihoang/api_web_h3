using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_WebH3.Models;

namespace API_WebH3.Models;

// UserNotification model to track notifications sent to users
public class UserNotification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey("Notification")]
    public Guid NotificationId { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public bool IsRead { get; set; } = false; // Track if the user has read the notification

    public string SentAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual Notification Notification { get; set; }
    public virtual User User { get; set; }
}