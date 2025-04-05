using API_WebH3.DTOs.Enrollment;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollementService _enrollementService;

    public EnrollmentController(EnrollementService enrollementService)
    {
        _enrollementService = enrollementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EnrollmentDto>>> GetAllAsync()
    {
        var enrollments = await _enrollementService.GetAllAsync();
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetById(int id)
    {
        var enrollment = await _enrollementService.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }
        return Ok(enrollment);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EnrollmentDto>> CreateAsync(CreateEnrollmentDto createEnrollmentDto)
    {
        // Kiểm tra xem người dùng đã đăng ký khóa học này chưa
        var existingEnrollment = await _enrollementService.GetByUserAndCourseAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId.ToString());
        if (existingEnrollment != null)
        {
            return BadRequest("Bạn đã đăng ký khóa học này rồi");
        }

        var enrollment = await _enrollementService.CreateAsync(createEnrollmentDto);
        return CreatedAtAction(nameof(GetById), new {id = enrollment.Id}, enrollment);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EnrollmentDto>> UpdateAsync(int id, CreateEnrollmentDto updateEnrollmentDto)
    {
        var enrollment = await _enrollementService.UpdateAsync(id, updateEnrollmentDto);
        if (enrollment == null)
        {
            return NotFound();
        }
        return Ok(enrollment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var enrollment = await _enrollementService.DeleteAsync(id);
        if (!enrollment)
        {
            return NotFound();
        }
        return Ok(enrollment);
    }

    [HttpGet("user/{userId}")]
    [Authorize] 
    public async Task<ActionResult<List<EnrollmentDto>>> GetByUserIdAsync(Guid userId)
    {
        var enrollments = await _enrollementService.GetByUserIdAsync(userId);
        return Ok(enrollments);
    }
}