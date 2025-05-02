using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Lesson;

public class UpdateLessonDto
{
    [Required]
    public Guid ChapterId { get; set; }
    [Required]
    public string CourseId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public string? Content { get; set; }
    public List<string>? VideoUrls { get; set; }
    public int Duration { get; set; }
    public int OrderNumber { get; set; }
}