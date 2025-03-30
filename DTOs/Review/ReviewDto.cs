namespace API_WebH3.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}