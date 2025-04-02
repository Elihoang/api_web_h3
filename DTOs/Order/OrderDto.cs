namespace API_WebH3.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<OrderDetailsDto>? OrderDetails { get; set; }
}