using API_WebH3.DTOs.Comment;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CommentController : Controller
{
    private readonly CommentService _commentService;

    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments()
    {
        var comments = await _commentService.GetAllComments();
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetCommentById(int id)
    {
        var comment = await _commentService.GetCommentById(id);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpGet("User/{id}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentByUserid(Guid userId)
    {
        var comment = await _commentService.GetCommentsByUserId(userId);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpGet("Post/{id}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentByPostid(Guid postId)
    {
        var comment = await _commentService.GetCommentsByPostId(postId);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var comment = await _commentService.CreateComment(createCommentDto);
        return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var comment = await _commentService.UpdateComment(id, updateCommentDto);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CommentDto>> DeleteComment(int id)
    {
        var comment = await _commentService.DeleteComment(id);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }
}