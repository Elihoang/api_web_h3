namespace API_WebH3.DTO.User;

public class UpdateStudentDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? ProfileImage { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
}