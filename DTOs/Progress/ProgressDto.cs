namespace API_WebH3.DTOs.Progress;

public class ProgressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public Guid LessonId { get; set; }
    
    public string Status { get; set; }
    public DateTime LastUpdate { get; set; }
}