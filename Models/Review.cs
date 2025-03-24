using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    
    [Range(1,5)]
    public int Rating { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; }=DateTime.Now;
    
    public virtual User User { get; set; }  
    
    public virtual Course Course { get; set; }
}