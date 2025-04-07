using API_WebH3.DTOs.Course;
using API_WebH3.DTOs.User;
using API_WebH3.Models;

namespace API_WebH3.DTOs.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; } 
        public string? Content { get; set; }

        public string CreatedAt { get; set; }
        public string? Tags { get; set; }
        public string? UrlImage { get; set; }
        public UserDTO User { get; set; } // Chứa toàn bộ thông tin User
    }

   
}
