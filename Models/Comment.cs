using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [ForeignKey("Post")]
    public Guid PostId { get; set; }
    
    [Required]
    public required string Content { get; set; }
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User User { get; set; }
    
    public virtual Post Post { get; set; }
}