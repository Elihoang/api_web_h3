using System.ComponentModel.DataAnnotations;

namespace API_WebH3.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
        public required string Role { get; set; } = "Student";
        public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        public string? IpAddress { get; set; }
        public string? DeviceName { get; set; }
    }
}
