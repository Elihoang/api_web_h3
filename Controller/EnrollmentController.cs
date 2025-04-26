using API_WebH3.DTO.Enrollment;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _enrollmentService;
    public EnrollmentController(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }
        return Ok(enrollment);
    }
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByUser(Guid userId)
    {
        var enrollments = await _enrollmentService.GetByUserIdAsync(userId);
        return Ok(enrollments);
    }
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourse(Guid courseId)
    {
        var enrollments = await _enrollmentService.GetByCourseIdAsync(courseId);
        return Ok(enrollments);
    }
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollment(CreateEnrollmentDto createEnrollmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var enrollmentDto = await _enrollmentService.CreateAsync(createEnrollmentDto);
        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollmentDto.Id }, enrollmentDto);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<EnrollmentDto>> UpdateEnrollment(int id, UpdateEnrollmentDto updateEnrollmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var enrollmentDto = await _enrollmentService.UpdateAsync(id, updateEnrollmentDto);
        if (enrollmentDto == null)
        { 
            return NotFound();
        }
        return Ok(enrollmentDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var result = await _enrollmentService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}