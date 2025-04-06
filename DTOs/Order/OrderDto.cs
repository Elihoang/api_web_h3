namespace API_WebH3.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; } // Thêm
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; } // Thêm
    public decimal Amount { get; set; } 
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}