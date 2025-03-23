using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Course
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required] 
    [Column(TypeName = "decimal(10,2)")]
    public Decimal Price { get; set; }
    [Required]
    public int Instructor_id { get; set; }
    [Required]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User Instructor { get; set; }
    
}