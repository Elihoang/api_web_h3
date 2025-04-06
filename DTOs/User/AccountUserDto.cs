namespace API_WebH3.DTOs.User
{
    public class AccountUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        
    }
}
