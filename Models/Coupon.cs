using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API_WebH3.Models;

// Coupon for discounts
public class Coupon
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public required string Code { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; } // e.g., 10.00 for 10%
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public int MaxUsage { get; set; } // Maximum number of times coupon can be used
    
    public int CurrentUsage { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
}