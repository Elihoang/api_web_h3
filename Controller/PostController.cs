using API_WebH3.DTO.Post;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;


[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsAll()
    {
        var posts = await _postService.GetAllAsync();
        return Ok(posts);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(Guid id)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }
    
    
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var postDto = await _postService.CreateAsync(createPostDto);
            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpPut("{id}")]
    public async Task<ActionResult<PostDto>> UpdatePost(Guid id, UpdatePostDto updatePostDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var postDto = await _postService.UpdateAsync(id, updatePostDto);
        if (postDto == null)
        {
            return NotFound();
        }
        return Ok(postDto);
    }
    
    
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var result = await _postService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
    
}
