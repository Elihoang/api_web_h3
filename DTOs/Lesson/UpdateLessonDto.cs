using Microsoft.Build.Framework;

namespace API_WebH3.DTOs.Lesson;

public class UpdateLessonDto
{
    [Required]
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? VideoUrl { get; set; }

}