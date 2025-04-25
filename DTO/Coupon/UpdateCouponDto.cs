namespace API_WebH3.DTO.Coupon;

public class UpdateCouponDto
{
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxUsage { get; set; }
    public bool IsActive { get; set; }
}