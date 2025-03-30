using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Lesson
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    
    [Required]
    public required string Title { get; set; }
    
    public string? Content { get; set; }
    
    public string? VideoUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual Course Course { get; set; }
    
}