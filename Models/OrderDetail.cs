using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;


// OrderDetail for detailed order information
public class OrderDetail
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [ForeignKey("Coupon")]
    public Guid? CouponId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? DiscountAmount { get; set; }
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual Order Order { get; set; }
    public virtual Course Course { get; set; }
    public virtual Coupon? Coupon { get; set; }
}
