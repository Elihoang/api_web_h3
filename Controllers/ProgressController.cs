using API_WebH3.DTOs.Progress;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Yêu cầu xác thực cho tất cả endpoint
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProgressDto>>> GetAllAsync()
    {
        var progresses = await _progressService.GetAllAsync();
        return Ok(progresses);
    }

    [HttpGet("{id}", Name = "GetProgressById")]
    public async Task<ActionResult<ProgressDto>> GetByIdAsync(Guid id)
    {
        var progress = await _progressService.GetByIdAsync(id);
        if (progress == null) return NotFound();

        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != progress.UserId) return Forbid();

        return Ok(progress);
    }

    [HttpPost]
    public async Task<ActionResult<ProgressDto>> CreateAsync([FromBody] CreateProgressDto createProgressDto)
    {
        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != createProgressDto.UserId) return Forbid();

        var progress = await _progressService.CreateAsync(createProgressDto);
        return CreatedAtRoute("GetProgressById", new { id = progress.Id }, progress);

    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProgressDto>> UpdateAsync(Guid id, [FromBody] UpdateProgressDto updateProgressDto)
    {
        var progress = await _progressService.GetByIdAsync(id);
        if (progress == null) return NotFound();

        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != progress.UserId) return Forbid();

        var updatedProgress = await _progressService.UpdateAsync(id, updateProgressDto);
        if (updatedProgress == null) return NotFound();
        return Ok(updatedProgress);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var result = await _progressService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ProgressDto>>> GetByUserIdAsync(Guid userId)
    {
        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != userId) return Forbid();

        var progresses = await _progressService.GetByUserIdAsync(userId);
        return Ok(progresses);
    }

    [HttpGet("lesson/{lessonId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<List<ProgressDto>>> GetByLessonIdAsync(Guid lessonId)
    {
        var progresses = await _progressService.GetByLessonIdAsync(lessonId);
        return Ok(progresses);
    }

    [HttpGet("user/{userId}/lesson/{lessonId}")]
    public async Task<ActionResult<ProgressDto>> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != userId) return Forbid();

        var progress = await _progressService.GetByUserAndLessonAsync(userId, lessonId);
        if (progress == null) return NotFound();
        return Ok(progress);
    }

    [HttpGet("course/{courseId}/user/{userId}")]
    public async Task<ActionResult<List<ProgressDto>>> GetByCourseIdAsync(Guid courseId, Guid userId)
    {
        var userIdFromToken = Guid.Parse(User.FindFirst("id")?.Value ?? "");
        if (userIdFromToken != userId) return Forbid();

        var progresses = await _progressService.GetByCourseIdAsync(courseId, userId);
        return Ok(progresses);
    }
}