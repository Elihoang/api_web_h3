using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.User;

public class CreateUserDto
{
    [Required]
    public string FullName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfileImage { get; set; }
    [Required]
    public string Role { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceName { get; set; }
    public string? GoogleId { get; set; }
    public bool IsGoogleAccount { get; set; }
}