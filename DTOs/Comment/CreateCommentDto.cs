namespace API_WebH3.DTOs.Comment;

public class CreateCommentDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
}