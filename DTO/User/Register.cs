using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.User;

public class Register
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string Password { get; set; }

    [Required] public string FullName { get; set; }

    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }
    public string? ProfileImage { get; set; }

    public string Role { get; set; } = "Student"; // Mặc định là Student
}