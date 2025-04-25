using API_WebH3.DTO.Message;

namespace API_WebH3.DTO.Chat;

public class ChatDto
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public string CreatedAt { get; set; }
    public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
}