using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models;

// Message model for individual messages within a chat
public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey("Chat")]
    public Guid ChatId { get; set; }

    [Required]
    [ForeignKey("Sender")]
    public Guid SenderId { get; set; }

    [Required]
    public required string Content { get; set; }

    public bool IsRead { get; set; } = false; // Track if the message has been read

    public string SentAt { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    public virtual Chat Chat { get; set; }
    public virtual User Sender { get; set; }
}