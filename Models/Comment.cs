using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("Post")]
    public int PostId { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
    
    public virtual Post Post { get; set; }
}