namespace API_WebH3.DTO.User;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string ResetCode { get; set; }
    public string NewPassword { get; set; }
}