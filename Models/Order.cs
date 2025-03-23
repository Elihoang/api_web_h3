using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public Decimal TotalAmount { get; set; }
    
    
    public string Status { get; set; } = "Pending";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
    
}