namespace API_WebH3.DTOs.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; } 

    public Guid PostId { get; set; }
    public string? PostTitle { get; set; }
    public string Content { get; set; }
    public string CreatedAt { get; set; }
    
    
}