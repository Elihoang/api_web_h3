namespace API_WebH3.DTO.Message;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public string SentAt { get; set; }
}