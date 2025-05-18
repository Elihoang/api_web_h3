using API_WebH3.DTO.Post;
using API_WebH3.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;
    private readonly PhotoService _photoService;

    public PostController(PostService postService, PhotoService photoService)
    {
        _photoService = photoService;
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
    [Authorize]
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

    [HttpPost("{postId}/upload-image")]
    [Authorize]
    public async Task<IActionResult> UploadPostImage(Guid postId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Không có tệp nào được tải lên.");

        var imageUrl = await _photoService.UploadImageAsync(file);
        if (imageUrl == null)
            return BadRequest("Tải ảnh không thành công.");

        var postDto = await _postService.UpdatePostImageAsync(postId, imageUrl);
        if (postDto == null)
            return NotFound("Không tìm thấy người dùng.");

        return Ok(new { ImageUrl = postDto.UrlImage });
    }
}