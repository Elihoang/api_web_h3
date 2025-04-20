namespace API_WebH3.DTO.LessonApproval;

public class LessonApprovalDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid AdminId { get; set; }
    public string Action { get; set; }
    public string? Comments { get; set; }
    public string CreatedAt { get; set; }
}