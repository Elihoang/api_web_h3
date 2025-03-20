namespace API_WebH3.DTOs.User
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
        public string Role { get; set; } = "Student";
    }

}
