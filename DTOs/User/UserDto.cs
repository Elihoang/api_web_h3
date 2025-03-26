namespace API_WebH3.DTOs.User
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ?ProfileImage { get; set; }
        public string Role { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CreatedAt { get; set; }
    }
}
