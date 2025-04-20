using API_WebH3.DTO.LessonApproval;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class LessonApprovalController : ControllerBase
{
    private readonly LessonApprovalService _lessonApprovalService;

    public LessonApprovalController(LessonApprovalService lessonApprovalService)
    {
        _lessonApprovalService = lessonApprovalService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonApprovalDto>>> GetLessonApprovals()
    {
        var lessonApprovals = await _lessonApprovalService.GetAllLessonApprovals();
        return Ok(lessonApprovals);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<LessonApprovalDto>> GetLessonApproval(Guid id)
    {
        var lessonApproval = await _lessonApprovalService.GetLessonApprovalById(id);
        if (lessonApproval == null)
        {
            return NotFound();
        }
        return Ok(lessonApproval);
    }
    [HttpPost]
    public async Task<ActionResult<LessonApprovalDto>> CreateLessonApproval(CreateLessonApprovalDto createLessonApprovalDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var lessonApprovalDto = await _lessonApprovalService.CreateLessonApproval(createLessonApprovalDto);
            return CreatedAtAction(nameof(GetLessonApproval), new { id = lessonApprovalDto.Id }, lessonApprovalDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<LessonApprovalDto>> UpdateLessonApproval(Guid id, UpdateLessonApprovalDto updateLessonApprovalDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var lessonApprovalDto = await _lessonApprovalService.UpdateLessonApproval(id, updateLessonApprovalDto);
            if (lessonApprovalDto == null)
            {
                return NotFound();
            }
            return Ok(lessonApprovalDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLessonApproval(Guid id)
    {
        var lessonApproval = await _lessonApprovalService.DeleteAsync(id);
        if (!lessonApproval)
        {
            return NotFound();
        }
        return NoContent();
    }
}