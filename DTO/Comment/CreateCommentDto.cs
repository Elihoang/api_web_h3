using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Comment;

public class CreateCommentDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid PostId { get; set; }
    [Required]
    public string Content { get; set; }
    public int? ParentCommentId { get; set; }
}