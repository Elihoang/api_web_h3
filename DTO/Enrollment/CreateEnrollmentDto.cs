using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Enrollment;

public class CreateEnrollmentDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string CourseId { get; set; }
    public string Status { get; set; }
}