namespace API_WebH3.DTOs.Lesson;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Content { get; set; }
    public string? VideoUrl { get; set; }
    public int Duration { get; set; }
    public int OrderNumber { get; set; }
    public string CreatedAt { get; set; }
}