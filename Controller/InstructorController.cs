using API_WebH3.DTO.User;
using API_WebH3.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[Route("api/[controller]")]
[ApiController]
public class InstructorController : ControllerBase
{
    private readonly InstructorService _instructorService;
    private readonly PhotoService _photoService;
    public InstructorController(InstructorService instructorService, PhotoService photoService)
    {
        _instructorService = instructorService;
        _photoService = photoService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAllInstructors()
    {
        var instructors = await _instructorService.GetAllInstructorsAsync();
        return Ok(instructors);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<InstructorDto>> GetInstructor(Guid id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }
        return Ok(instructor);
    }
    [HttpPost]
    public async Task<ActionResult<InstructorDto>> CreateInstructor([FromBody] CreateInstructorDto createInstructorDto)
    {
        try
        {
            var instructor = await _instructorService.CreateInstructorAsync(createInstructorDto);
            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.Id }, instructor);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<InstructorDto>> UpdateInstructor(Guid id, [FromBody] UpdateInstructorDto updateInstructorDto)
    {
        try
        {
            var instructor = await _instructorService.UpdateInstructorAsync(id, updateInstructorDto);
            if (instructor == null)
            {
                return NotFound();
            }
            return Ok(instructor);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInstructor(Guid id)
    {
        var result = await _instructorService.DeleteInstructorAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
    [HttpPost("upload-image")]
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