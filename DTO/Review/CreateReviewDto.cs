using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Review;

public class CreateReviewDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid CourseId { get; set; }
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
}