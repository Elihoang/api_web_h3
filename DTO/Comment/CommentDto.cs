namespace API_WebH3.DTO.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } // Tên đầy đủ của người dùng
    public string UserProfileImage { get; set; } // Đường dẫn ảnh đại diện
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public int? ParentCommentId { get; set; }
    public string? ParentUserFullName { get; set; }
    public string CreatedAt { get; set; }
    public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
}