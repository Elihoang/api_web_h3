namespace API_WebH3.DTOs.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    
    
}