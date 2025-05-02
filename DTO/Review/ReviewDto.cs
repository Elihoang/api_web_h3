namespace API_WebH3.DTO.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string CreatedAt { get; set; }
}