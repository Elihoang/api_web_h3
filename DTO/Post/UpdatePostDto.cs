namespace API_WebH3.DTO.Post;

public class UpdatePostDto
{
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Tags { get; set; }
    public string? UrlImage { get; set; }
}