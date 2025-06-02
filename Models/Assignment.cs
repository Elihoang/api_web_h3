using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models
{
    public class Assignment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Tạo ID ngẫu nhiên

        [Required]
        public string Title { get; set; } // Tiêu đề bài tập

        public string Description { get; set; } // Mô tả bài tập

        public string? FileUrl { get; set; } // Đường dẫn đến file bài tập (nếu có)

        [Required]
        public Guid InstructorId { get; set; } // Liên kết với giáo viên (User với Role = Teacher)

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; } // Navigation property đến User

        [Required]
        public string LessonId { get; set; } // Liên kết với bài học (thay vì khóa học)

        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; } // Navigation property đến Lesson

        public Guid? StudentId { get; set; } // Liên kết với học sinh (null nếu gửi cho nhiều học sinh)

        [ForeignKey("StudentId")]
        public User? Student { get; set; } // Navigation property đến User (học sinh)

        public string? GroupId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian tạo bài tập

        public DateTime? Deadline { get; set; } // Hạn nộp bài tập

        public string Status { get; set; } = "Pending"; // Trạng thái: Pending, Sent, Completed
    }
}