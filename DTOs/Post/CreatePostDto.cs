namespace API_WebH3.DTOs.Post
{
    public class CreatePostDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? Tags { get; set; }
        public string? UrlImage { get; set; }
    }
}
