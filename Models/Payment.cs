using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; } // Phương thức thanh toán (VNPay, MoMo, COD...)
    
    [Required]
    public string Status { get; set; } = "pending"; // Trạng thái: pending, completed, failed, refunded
    
    [Required]
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User User { get; set; }
    
    public virtual Course Course { get; set; }
} 