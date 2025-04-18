using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;

// Chapter for organizing course content
public class Chapter
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    
    [Required]
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public int OrderNumber { get; set; } // Order within the course
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Course Course { get; set; }
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}