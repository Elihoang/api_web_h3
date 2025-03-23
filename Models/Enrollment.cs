using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("Course")]
    public int CourseId { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
    
    public virtual Course Course { get; set; }
    
}