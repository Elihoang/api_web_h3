using API_WebH3.DTOs.Progress;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    /// <summary>
    /// Lấy tất cả tiến độ học tập
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProgressDto>>> GetAllAsync()
    {
        var progresses = await _progressService.GetAllAsync();
        return Ok(progresses);
    }

    /// <summary>
    /// Lấy tiến độ học tập theo ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ProgressDto>> GetByIdAsync(Guid id)
    {
        var progress = await _progressService.GetByIdAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    /// <summary>
    /// Tạo tiến độ học tập mới
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProgressDto>> CreateAsync(CreateProgressDto createProgressDto)
    {
        var progress = await _progressService.CreateAsync(createProgressDto);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = progress.Id }, progress);
    }

    /// <summary>
    /// Cập nhật tiến độ học tập
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProgressDto>> UpdateAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        var progress = await _progressService.UpdateAsync(id, updateProgressDto);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    /// <summary>
    /// Xóa tiến độ học tập
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var result = await _progressService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Lấy tiến độ học tập của một người dùng
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<ProgressDto>>> GetByUserIdAsync(Guid userId)
    {
        var progresses = await _progressService.GetByUserIdAsync(userId);
        return Ok(progresses);
    }

    /// <summary>
    /// Lấy tiến độ học tập của một bài học
    /// </summary>
    [HttpGet("lesson/{lessonId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<List<ProgressDto>>> GetByLessonIdAsync(Guid lessonId)
    {
        var progresses = await _progressService.GetByLessonIdAsync(lessonId);
        return Ok(progresses);
    }

    /// <summary>
    /// Lấy tiến độ học tập của một người dùng trong một bài học cụ thể
    /// </summary>
    [HttpGet("user/{userId}/lesson/{lessonId}")]
    [Authorize]
    public async Task<ActionResult<ProgressDto>> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        var progress = await _progressService.GetByUserAndLessonAsync(userId, lessonId);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    /// <summary>
    /// Lấy tiến độ học tập của một người dùng trong một khóa học
    /// </summary>
    [HttpGet("course/{courseId}/user/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<ProgressDto>>> GetByCourseIdAsync(Guid courseId, Guid userId)
    {
        var progresses = await _progressService.GetByCourseIdAsync(courseId, userId);
        return Ok(progresses);
    }
}