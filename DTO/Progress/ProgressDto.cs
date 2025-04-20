namespace API_WebH3.DTO.Progress;

public class ProgressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }
    public string Status { get; set; }
    public int CompletionPercentage { get; set; }
    public string? Notes { get; set; }
    public string LastUpdate { get; set; }
}