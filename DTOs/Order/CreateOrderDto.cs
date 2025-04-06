namespace API_WebH3.DTOs.Order;

public class CreateOrderDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";

}