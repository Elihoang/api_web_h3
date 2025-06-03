using API_WebH3.DTO.Comment;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentService _commentService;
    
    
    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
    {
        var comments = await _commentService.GetAllAsync();
        return Ok(comments);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }
    
    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPost(Guid postId)
    {
        var comments = await _commentService.GetByPostIdAsync(postId);
        return Ok(comments);
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var commentDto = await _commentService.CreateAsync(createCommentDto);
        return CreatedAtAction(nameof(GetComment), new { id = commentDto.Id }, commentDto);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int id, UpdateCommentDto updateCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var commentDto = await _commentService.UpdateAsync(id, updateCommentDto);
        if (commentDto == null)
        {
            return NotFound();
        }
        return Ok(commentDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
    [HttpGet("paginated")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPaginatedComment([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var comment = await _commentService.GetAllAsync();
        var totalItems = comment.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var pagedCommentList = comment.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new
        {
            Data = pagedCommentList,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        return Ok(result);
    }
}