using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class OrderDetails
{
    public Guid Id { get; set; }
    
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public Decimal  Price { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual Order Order { get; set; }
    
    public virtual Course Course { get; set; }
}