namespace API_WebH3.DTO.User;

public class InstructorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfileImage { get; set; }
    public string Role { get; set; } = "Instructor";
    public string CreatedAt { get; set; }
 
}