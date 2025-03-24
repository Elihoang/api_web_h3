using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Course
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public string? UrlImage { get; set; }

    [Required]
    [ForeignKey("User")] // Khớp với navigation property
    public required Guid InstructorId { get; set; }  // Đặt tên là UserId để phản ánh rõ ràng

    [Required]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual User User { get; set; } // Định danh mối quan hệ với User
}
