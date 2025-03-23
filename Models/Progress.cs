using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Progress
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("Lesson")]
    public int LessonId { get; set; }
    
    public string Status { get; set; } = "not started";
    
    public DateTime LastUpdate { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
    
    public virtual Lesson Lesson { get; set; }
    
}