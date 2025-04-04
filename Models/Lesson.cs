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
    
    [Required]
    public required string Description { get; set; }
    
    public string? Content { get; set; }
    
    public string? VideoUrl { get; set; }
    
    public int Duration { get; set; } // Thời lượng bài học (phút)
    
    public int OrderNumber { get; set; } // Số thứ tự bài học trong khóa học
    
    [Required]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Course Course { get; set; }
}