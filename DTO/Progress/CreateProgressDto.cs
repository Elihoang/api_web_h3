using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Progress;

public class CreateProgressDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid LessonId { get; set; }
    [Required]
    public string Status { get; set; }
    public int CompletionPercentage { get; set; } = 0;
    public string? Notes { get; set; }
}