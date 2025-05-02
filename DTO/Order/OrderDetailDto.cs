using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Order;

public class OrderDetailDto
{
    [Required]
    public string CourseId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    public Guid? CouponId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm giá phải lớn hơn hoặc bằng 0")]
    public decimal? DiscountAmount { get; set; }
}