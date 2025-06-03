using System.Security.Claims;
using API_WebH3.DTO.Notification;
using API_WebH3.DTO.UserNotification;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;
using CreateNotificationDto = API_WebH3.DTO.Notification.CreateNotificationDto;

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
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByUser(Guid userId)
    {
        var notifications = await _notificationService.GetByUserIdAsync(userId);
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
    
    [HttpPut("{notificationId}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, [FromBody] Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "UserId không hợp lệ" });
            }
            await _notificationService.MarkAsReadAsync(notificationId, userId);
            return Ok(new { message = "Đánh dấu thông báo là đã đọc thành công" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi đánh dấu thông báo", error = ex.Message });
        }
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
    
    [HttpGet("paginated")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetPaginatedNotification([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var notification = await _notificationService.GetAllAsync();
        var totalItems = notification.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var pagedNotificationList = notification.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new
        {
            Data = pagedNotificationList,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        return Ok(result);
    }

}