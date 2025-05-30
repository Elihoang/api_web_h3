namespace API_WebH3.DTO.Enrollment;

public class EnrollmentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}