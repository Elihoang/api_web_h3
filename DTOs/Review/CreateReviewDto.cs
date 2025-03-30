namespace API_WebH3.DTOs.Review;

public class CreateReviewDto
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    
}