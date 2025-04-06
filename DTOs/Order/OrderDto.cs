namespace API_WebH3.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
    
  
}