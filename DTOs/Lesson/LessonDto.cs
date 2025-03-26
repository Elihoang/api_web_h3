namespace API_WebH3.DTOs.Lesson;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? VideoUrl { get; set; } 
    public DateTime CreatedAt { get; set; }
}