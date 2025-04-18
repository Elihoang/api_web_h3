using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

public class Chat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey("User1")]
    public Guid User1Id { get; set; } // One participant (e.g., Teacher or Student)

    [Required]
    [ForeignKey("User2")]
    public Guid User2Id { get; set; } // Other participant (e.g., Student or Teacher)

    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual User User1 { get; set; }
    public virtual User User2 { get; set; }
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
