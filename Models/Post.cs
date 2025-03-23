using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    public string? Content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
}