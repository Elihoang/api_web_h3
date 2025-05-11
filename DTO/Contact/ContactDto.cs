namespace API_WebH3.DTO.Contact;

public class ContactDto
{

    public string SenderEmail { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
}