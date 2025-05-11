namespace API_WebH3.DTO.User;

public class StudentDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? BirthDate { get; set; }
    public string? ProfileImage { get; set; }
    public string Role { get; set; }
    public string CreatedAt { get; set; }
}