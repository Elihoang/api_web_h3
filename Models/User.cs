﻿using System.ComponentModel.DataAnnotations;

namespace API_WebH3.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; } // Có thể null cho người dùng đăng nhập bằng Google
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
        public required string Role { get; set; } //Admin, Student, Teacher
        public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        public string? IpAddress { get; set; }
        public string? DeviceName { get; set; }
        public string? GoogleId { get; set; } // ID người dùng từ Google
        public bool IsGoogleAccount { get; set; } = false; // Flag để xác định tài khoản Google
    }
}