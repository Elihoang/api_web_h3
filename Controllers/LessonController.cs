using API_WebH3.DTOs.Lesson;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LessonController : ControllerBase
{
    private readonly LessonService _lessonService;

    public LessonController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Lấy tất cả các bài học
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<LessonDto>>> GetAllAsync()
    {
        var lessons = await _lessonService.GetAllAsync();
        return Ok(lessons);
    }

    /// <summary>
    /// Lấy bài học theo Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetByIdAsync(Guid id)
    {
        var lesson = await _lessonService.GetByIdAsync(id);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }

    /// <summary>
    /// Tạo mới bài học (Chỉ Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> CreateAsync(CreateLessonDto createLessonDto)
    {
        var lesson = await _lessonService.CreateAsync(createLessonDto);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = lesson.Id }, lesson);
    }

    /// <summary>
    /// Cập nhật bài học theo Id (Chỉ Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> UpdateAsync(Guid id, UpdateLessonDto updateLessonDto)
    {
        var lesson = await _lessonService.UpdateAsync(id, updateLessonDto);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }

    /// <summary>
    /// Xóa bài học theo Id (Chỉ Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var result = await _lessonService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách bài học theo khóa học
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<LessonDto>>> GetByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonService.GetByCourseIdAsync(courseId);
        return Ok(lessons);
    }

    /// <summary>
    /// Lấy bài học theo khóa học và số thứ tự
    /// </summary>
    [HttpGet("course/{courseId}/order/{orderNumber}")]
    public async Task<ActionResult<LessonDto>> GetByCourseIdAndOrderAsync(Guid courseId, int orderNumber)
    {
        var lesson = await _lessonService.GetByCourseIdAndOrderAsync(courseId, orderNumber);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }
} 