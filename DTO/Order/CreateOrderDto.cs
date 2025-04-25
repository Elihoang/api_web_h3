using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Order;

public class CreateOrderDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Tổng số tiền phải lớn hơn hoặc bằng 0")]
    public decimal Amount { get; set; }

    public string Status { get; set; } = "Pending";

    public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
}