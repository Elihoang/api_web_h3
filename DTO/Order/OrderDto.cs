using API_WebH3.Models;

namespace API_WebH3.DTO.Order;

public class OrderDto
{
    public string Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
    public Models.User User { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}