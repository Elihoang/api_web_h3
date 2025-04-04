using API_WebH3.DTOs.Lesson;
using API_WebH3.Models;
using API_WebH3.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LessonsController : ControllerBase
{
    
    private readonly LessonService _lessonService;

    public LessonsController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }
    
    //Lấy tất cả bài học
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetAllLessons()
    {
        var lessons = await _lessonService.GetAllLessonAsync();
        return Ok(lessons);
    }
    //lấy bài học theo id

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }
    //lấy bài học theo id khóa 
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourseId(Guid courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        if (lessons == null)
        {
            return NotFound();
        }

        return Ok(lessons);
    }
    
    //Thêm bài học 
    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto createLessonDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var lesson = await _lessonService.CreateLessonAsync(createLessonDto);
        return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lesson);
    }
    //Update bài học
    [HttpPut("{id}")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(Guid id, [FromBody] UpdateLessonDto updateLessonDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var lesson = await _lessonService.UpdateLessonAsync(id, updateLessonDto);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok(lesson);
    }
    // Xóa bài học
    [HttpDelete("{id}")]
    public async Task<ActionResult<LessonDto>> DeleteLesson(Guid id)
    {
        var lesson = await _lessonService.DeleteLessonAsync(id);
        if (lesson == null)
        {
            return NotFound();
        }
        return Ok();
    }
}