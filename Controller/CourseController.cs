using API_WebH3.DTO.Course;
using API_WebH3.Service;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly CourseService _courseService;
    private readonly PhotoService _photoService;
    public CourseController(CourseService courseService, PhotoService photoService)
    {
        _courseService = courseService;
        _photoService = photoService;
    }

    [HttpGet("paginated")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetPaginatedCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var courses = await _courseService.GetAllAsync();
        var totalItems = courses.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var pagedCourseList = courses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new
        {
            Data = pagedCourseList,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _courseService.GetAllAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(string id)
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

        var courseDto = await _courseService.CreateCourseAsync(createCourseDto);
        return CreatedAtAction(nameof(GetCourse), new { id = courseDto.Id }, courseDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(string id, UpdateCourseDto updateCourseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var courseDto = await _courseService.UpdateCourseAsync(id, updateCourseDto);
        if (courseDto == null)
        {
            return NotFound();
        }
        return Ok(courseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(string id)
    {
        var course = await _courseService.DeleteAsync(id);
        if (!course)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByCategoryId(string categoryId)
    {
        var courses = await _courseService.GetByCategoryIdAsync(categoryId);
        return Ok(courses);
    }

    [HttpGet("instructor/{instructorId}")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByInstructorId(string instructorId)
    {
        var courses = await _courseService.GetByInstructorIdAsync(instructorId);

        return Ok(courses);
    }

    [HttpPost("upload-image")]
    // [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var imageUrl = await _photoService.UploadImageAsync(file);
        if (imageUrl == null)
            return BadRequest("Upload failed.");

        return Ok(new { url = imageUrl });
    }

}