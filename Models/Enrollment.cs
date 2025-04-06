using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Enrolled"; // Enrolled, Completed, Failed

    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual User User { get; set; }
    
    public virtual Course Course { get; set; }
    
}