namespace API_WebH3.DTOs.Order;

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public List<OrderDetailsDto> OrderDetails { get; set; }
}