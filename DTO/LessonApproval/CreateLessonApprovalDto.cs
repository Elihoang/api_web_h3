using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.LessonApproval;

public class CreateLessonApprovalDto
{
    [Required]
    public string LessonId { get; set; }
    [Required]
    public Guid AdminId { get; set; }
    [Required]
    public string Action { get; set; }
    public string? Comments { get; set; }
}