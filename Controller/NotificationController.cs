using API_WebH3.DTO.Notification;
using API_WebH3.DTO.UserNotification;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;
    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
    {
        var notifications = await _notificationService.GetAllAsync();
        return Ok(notifications);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
    {
        var notification = await _notificationService.GetByIdAsync(id);
        if (notification == null)
        {
            return NotFound();
        }
        return Ok(notification);
    }
    [HttpPost]
    public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createNotificationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var notificationDto = await _notificationService.CreateAsync(createNotificationDto);
        return CreatedAtAction(nameof(GetNotification), new { id = notificationDto.Id }, notificationDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var result = await _notificationService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}