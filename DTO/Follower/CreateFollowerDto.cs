using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Follower;

public class CreateFollowerDto
{
    [Required]
    public Guid FollowerId { get; set; }
    [Required]
    public Guid FollowingId { get; set; }
}