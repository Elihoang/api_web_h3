namespace API_WebH3.DTOs.Enrollment;

public class EnrollmentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }    
    public Guid CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; }
}