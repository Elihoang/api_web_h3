using API_WebH3.DTO.Progress;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

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
    [HttpPost]
    public async Task<ActionResult<ProgressDto>> CreateProgress(CreateProgressDto createProgressDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var progressDto = await _progressService.CreateAsync(createProgressDto);
            return CreatedAtAction(nameof(GetProgress), new { id = progressDto.Id }, progressDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProgressDto>> UpdateProgress(Guid id, UpdateProgressDto updateProgressDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var progressDto = await _progressService.UpdateAsync(id, updateProgressDto);
            if (progressDto == null)
            {
                return NotFound();
            }
            return Ok(progressDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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