using API_WebH3.DTO.Chapter;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class ChapterController : ControllerBase
{
    private readonly ChapterService _chapterService;

    public ChapterController(ChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChapterDto>>> GetChaptersAll()
    {
        var chapters = await _chapterService.GetAllChapters();
        return Ok(chapters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChapterDto>> GetChapterById(Guid id)
    {
        var chapter = await _chapterService.GetChapterById(id);
        if (chapter == null)
        {
            return NotFound();
        }
        return Ok(chapter);
    }

    [HttpPost]
    public async Task<ActionResult<ChapterDto>> CreateChapter(CreateChapterDto createChapterDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var  chapter = await _chapterService.CreateChapter(createChapterDto);
        return CreatedAtAction(nameof(GetChapterById), new { id = chapter.Id }, chapter);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ChapterDto>> UpdateChapter(Guid id, UpdateChapterDto updateChapterDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var chapter = await _chapterService.UpdateChapter(id, updateChapterDto);
        if (chapter == null)
        {
            return NotFound();
        }
        return Ok(chapter);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChapter(Guid id)
    {
        var chapter = await _chapterService.DeleteChapter(id);
        if (!chapter)
        {
            return NotFound();
        }
        return NoContent();
    }
    
}