namespace API_WebH3.DTOs.Enrollment;

public class CreateEnrollmentDto
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
}