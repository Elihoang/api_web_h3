using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_WebH3.Helpers;

namespace API_WebH3.Models;

public class Order
{
    [Key] 
    public string Id { get; set; } = IdGenerator.IdOrder();
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public Decimal Amount { get; set; }
    
    public string Status { get; set; } = "Pending";
   
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    
    public virtual User User { get; set; }
    public virtual Course Course { get; set; }

}