
using System.ComponentModel.DataAnnotations;

namespace API_WebH3.DTO.Chat;

public class CreateChatDto
{
    [Required]
    public Guid User1Id { get; set; }
    [Required]
    public Guid User2Id { get; set; }
}