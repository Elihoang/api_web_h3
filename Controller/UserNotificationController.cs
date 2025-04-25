using API_WebH3.DTO.UserNotification;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserNotificationController : ControllerBase
{
    private readonly UserNotificationService _userNotificationService;
    public UserNotificationController(UserNotificationService userNotificationService)
    {
        _userNotificationService = userNotificationService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserNotificationDto>>> GetUserNotifications()
    {
        var userNotifications = await _userNotificationService.GetAllAsync();
        return Ok(userNotifications);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<UserNotificationDto>> GetUserNotification(Guid id)
    {
        var userNotification = await _userNotificationService.GetByIdAsync(id);
        if (userNotification == null)
        {
            return NotFound();
        }
        return Ok(userNotification);
    }
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserNotificationDto>>> GetUserNotificationsByUser(Guid userId)
    {
        var userNotifications = await _userNotificationService.GetByUserIdAsync(userId);
        return Ok(userNotifications);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<UserNotificationDto>> UpdateUserNotification(Guid id, UpdateUserNotificationDto updateUserNotificationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userNotificationDto = await _userNotificationService.UpdateAsync(id, updateUserNotificationDto);
        if (userNotificationDto == null)
        {
            return NotFound();
        }
        return Ok(userNotificationDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserNotification(Guid id)
    {
        var result = await _userNotificationService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
