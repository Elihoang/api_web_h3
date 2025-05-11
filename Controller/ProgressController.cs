using API_WebH3.DTO.Progress;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;
    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgressDto>>> GetProgresses()
    {
        var progresses = await _progressService.GetAllAsync();
        return Ok(progresses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProgressDto>> GetProgress(Guid id)
    {
        var progress = await _progressService.GetByIdAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    [HttpGet("user/{userId}/lesson/{lessonId}")]
    public async Task<ActionResult> GetProgressByUserAndLesson(Guid userId, string lessonId)
    {
        Console.WriteLine($"Yêu cầu lấy tiến độ: userId={userId}, lessonId={lessonId}");
        var progress = await _progressService.GetByUserAndLessonAsync(userId, lessonId);
        if (progress == null)
        {
            Console.WriteLine($"Không tìm thấy tiến độ: userId={userId}, lessonId={lessonId}");
            return NotFound();
        }
        Console.WriteLine($"Tiến độ tìm thấy: {JsonSerializer.Serialize(progress)}");
        return Ok(progress);
    }

    [HttpPost]
    public async Task<ActionResult<ProgressDto>> CreateProgress(CreateProgressDto createProgressDto)
    {
        Console.WriteLine($"Yêu cầu tạo tiến độ: {JsonSerializer.Serialize(createProgressDto)}");
        if (!ModelState.IsValid)
        {
            Console.WriteLine($"ModelState không hợp lệ: {JsonSerializer.Serialize(ModelState)}");
            return BadRequest(ModelState);
        }

        try
        {
            var progressDto = await _progressService.CreateAsync(createProgressDto);
            Console.WriteLine($"Tiến độ đã tạo: {JsonSerializer.Serialize(progressDto)}");
            return CreatedAtAction(nameof(GetProgress), new { id = progressDto.Id }, progressDto);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Lỗi ArgumentException: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi không mong muốn: {ex.Message}");
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProgressDto>> UpdateProgress(Guid id, UpdateProgressDto updateProgressDto)
    {
        Console.WriteLine($"Yêu cầu cập nhật tiến độ: id={id}, data={JsonSerializer.Serialize(updateProgressDto)}");
        if (!ModelState.IsValid)
        {
            Console.WriteLine($"ModelState không hợp lệ: {JsonSerializer.Serialize(ModelState)}");
            return BadRequest(ModelState);
        }

        try
        {
            var progressDto = await _progressService.UpdateAsync(id, updateProgressDto);
            if (progressDto == null)
            {
                Console.WriteLine($"Không tìm thấy tiến độ với id={id}");
                return NotFound();
            }
            Console.WriteLine($"Tiến độ đã cập nhật: {JsonSerializer.Serialize(progressDto)}");
            return Ok(progressDto);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Lỗi ArgumentException: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi không mong muốn: {ex.Message}");
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProgress(Guid id)
    {
        var progress = await _progressService.DeleteAsync(id);
        if (!progress)
        {
            return NotFound();
        }
        return NoContent();
    }
}