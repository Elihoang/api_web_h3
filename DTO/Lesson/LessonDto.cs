namespace API_WebH3.DTO.Lesson;

public class LessonDto
{
    public string Id { get; set; }
    public string ChapterId { get; set; }
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Content { get; set; } 
    public string VideoName { get; set; }
    public int Duration { get; set; }
    public int OrderNumber { get; set; }
    public string Status { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string CreatedAt { get; set; }
}