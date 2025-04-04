using API_WebH3.DTOs.Progress;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProgressDto>>> GetAll()
    {
        var progress = await _progressService.GetAllAsync();
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProgressDto>> GetById(Guid id)
    {
        var progress = await _progressService.GetByIdAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }
    [HttpPost]
    public async Task<ActionResult<ProgressDto>> CreateAsync(CreateProgressDto createProgressDto)
    {
        
        try
        {
            var progress = await _progressService.CreateAsync(createProgressDto);
            return CreatedAtAction(nameof(GetById), new { id = progress.Id }, progress);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred while creating progress", detail = ex.Message });
        }
        
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProgressDto>> UpdateAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        var progress = await _progressService.UpdateAsync(id, updateProgressDto);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProgressDto>> Delete(Guid id)
    {
        var progress = await _progressService.DeleteAsync(id);
        if (!progress)
        {
            return NotFound();
        }
        return Ok(progress);
    }
}