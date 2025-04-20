using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Review;

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
}