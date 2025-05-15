using API_WebH3.DTO.Lesson;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly LessonService _lessonService;

    public LessonController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsAll()
    {
        var lesson = await _lessonService.GetAllLessons();
        return Ok(lesson);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLessonById(string id)
    {
        var lesson = await _lessonService.GetLessonById(id);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }

    [HttpGet("chapter/{chapterId}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByChapterId(string chapterId)
    {
        var lessons = await _lessonService.GetLessonsByChapterId(chapterId);
        return Ok(lessons);
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourseId(string courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseId(courseId);
        return Ok(lessons);
    }

    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonDto createLessonDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var lesson = await _lessonService.CreateLesson(createLessonDto);
        return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateLesson(string id, UpdateLessonDto updateLessonDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var lesson = await _lessonService.UpdateLesson(id, updateLessonDto);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLesson(string id)
    {
        var lesson = await _lessonService.DeleteLesson(id);
        if (!lesson)
        {
            return NotFound(); 
        }
        return NoContent();
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File không hợp lệ");

        var path = Path.Combine("wwwroot/videos", file.FileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"{Request.Scheme}://{Request.Host}/videos/{file.FileName}";
        return Ok(new { url });
    }

}