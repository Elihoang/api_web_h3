using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

// Modified Comment with Reply functionality
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
    
    [ForeignKey("ParentComment")]
    public int? ParentCommentId { get; set; }

    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User User { get; set; }
    
    public virtual Post Post { get; set; }
    
    public virtual Comment? ParentComment { get; set; }
    
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public string? ReactionIcon { get; set; } 
}