using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Coupon;

public class CreateCouponDto
{
    [Required]
    public string Code { get; set; }
    [Required]
    public decimal DiscountPercentage { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public int MaxUsage { get; set; }
    public bool IsActive { get; set; } = true;
}