namespace API_WebH3.DTOs.Order;

public class OrderDetailsDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Price { get; set; }
    public string CreatedAt { get; set; }
}