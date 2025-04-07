using API_WebH3.DTOs.Post;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound(new { message = "Post not found" });
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
        {
            if (postDto == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            try
            {
                var createdPost = await _postService.AddPostAsync(postDto);
                return CreatedAtAction(nameof(GetAllPosts), new { id = createdPost.Id }, createdPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] CreatePostDto postDto)
        {
            if (postDto == null)
                return BadRequest(new { message = "Invalid post data" });
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
                return NotFound(new { message = "Không tìm thấy bài viết" });
            await _postService.UpdatePostAsync(id, postDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
                return NotFound(new { message = "Không tìm thấy bài viết" });
            await _postService.DeletePostAsync(id);
            return NoContent();
        }

        [HttpPost("upload-image/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(string id, IFormFile file)
        {
            var imageUrl = await _postService.UploadImageAsync(Guid.Parse(id), file);
            return Ok(new { message = "Upload image successfully!", imageUrl });
        }
    }
}
