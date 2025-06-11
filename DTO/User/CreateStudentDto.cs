namespace API_WebH3.DTO.User;

public class CreateStudentDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Role { get; set; } = "Student";
    public string? ProfileImage { get; set; }
    public string? Phone { get; set; }
}