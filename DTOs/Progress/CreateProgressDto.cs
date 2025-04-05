namespace API_WebH3.DTOs.Progress;

public class CreateProgressDto
{
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }
    public string Status { get; set; } = "not started";
    public int CompletionPercentage { get; set; } = 0;
    public string? Notes { get; set; }
}