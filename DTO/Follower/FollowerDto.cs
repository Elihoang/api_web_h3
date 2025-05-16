namespace API_WebH3.DTO.Follower;

public class FollowerDto
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public string FollowerFullName { get; set; } 
    public string FollowerEmail { get; set; }   
    public Guid FollowingId { get; set; }
    public string FollowingFullName { get; set; } 
    public string FollowingEmail { get; set; }  
    public string CreatedAt { get; set; }
}