using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Lesson;

public class CreateLessonDto
{
    [Required]
    public string ChapterId { get; set; }
    [Required]
    public string CourseId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public string? Content { get; set; }
    public string VideoName { get; set; }
    public int Duration { get; set; }
    public int OrderNumber { get; set; }
}