namespace API_WebH3.DTO.Coupon;

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxUsage { get; set; }
    public int CurrentUsage { get; set; }
    public bool IsActive { get; set; }
    public string CreatedAt { get; set; }
}