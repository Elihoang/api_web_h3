using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Post;

public class CreatePostDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Tags { get; set; }
    public string? UrlImage { get; set; }
}