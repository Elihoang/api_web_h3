using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Progress
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [ForeignKey("Lesson")] 
    public string LessonId { get; set; }
    
    [Required]
    public string Status { get; set; } = "not started"; // Trạng thái: not started, in progress, completed
    
    public int CompletionPercentage { get; set; } = 0; // Phần trăm hoàn thành
    
    public string? Notes { get; set; } // Ghi chú của người học
    
    [Required]
    public string LastUpdate { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User User { get; set; }
    
    public virtual Lesson Lesson { get; set; }
}