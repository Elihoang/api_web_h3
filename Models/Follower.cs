using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

// Follower model to manage follow relationships between users
public class Follower
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey("FollowerUser")]
    public Guid FollowerId { get; set; } // User who is following

    [Required]
    [ForeignKey("FollowingUser")]
    public Guid FollowingId { get; set; } // User being followed

    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual User FollowerUser { get; set; }
    public virtual User FollowingUser { get; set; }
}