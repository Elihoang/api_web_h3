using API_WebH3.DTO.User;

namespace API_WebH3.DTO.Post;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Tags { get; set; }
    public string? UrlImage { get; set; }
    public string CreatedAt { get; set; }
    public UserDto User { get; set; } 
}