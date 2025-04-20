using API_WebH3.DTO.Course;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly CourseService _courseService;
    public CourseController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _courseService.GetAllAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var courseDto = await _courseService.CreateChapter(createCourseDto);
        return CreatedAtAction(nameof(GetCourse), new { id = courseDto.Id }, courseDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(Guid id, UpdateCourseDto updateCourseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var courseDto = await _courseService.UpdateChapter(id, updateCourseDto);
        if (courseDto == null)
        {
            return NotFound();
        }
        return Ok(courseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var course = await _courseService.DeleteAsync(id);
        if (!course)
        {
            return NotFound();
        }
        return NoContent();
    }
}