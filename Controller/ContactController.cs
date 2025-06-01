using API_WebH3.DTO.Contact;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API_WebH3.Services;

[Route("api/contact")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ContactEmailService _contactEmailService;

    public ContactController(ContactEmailService contactEmailService)
    {
        _contactEmailService = contactEmailService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] ContactDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SenderEmail) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Thiếu thông tin!");
        }

        try
        {
            await _contactEmailService.SendEmailAsync(request.SenderEmail, request.Subject, request.Message);
            return Ok(new { message = "Gửi thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Lỗi: {ex.Message}" });
        }
    }
}