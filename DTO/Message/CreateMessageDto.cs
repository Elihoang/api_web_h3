using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Message;

public class CreateMessageDto
{
    [Required]
    public Guid ChatId { get; set; }
    [Required]
    public Guid SenderId { get; set; }
    [Required]
    public string Content { get; set; }
}