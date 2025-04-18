using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;

public class LessonApproval
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey("Lesson")]
    public Guid LessonId { get; set; }
    
    [Required]
    [ForeignKey("Admin")]
    public Guid AdminId { get; set; }
    
    [Required]
    public string Action { get; set; } = "Pending"; // Approved, Rejected
    
    public string? Comments { get; set; } // Reason for rejection or additional notes
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Lesson Lesson { get; set; }
    public virtual User Admin { get; set; }
}