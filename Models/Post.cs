using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [Required]
    public required string Title { get; set; }
    
    public string? Content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
}