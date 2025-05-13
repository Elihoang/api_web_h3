using API_WebH3.DTO.Course;
using API_WebH3.DTO.Post;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    
    private readonly CourseService _courseService;
        private readonly PostService _postService;

        public SearchController(CourseService courseService, PostService postService)
        {
            _courseService = courseService;
            _postService = postService;
        }

        // ✅ Tìm kiếm khóa học
        [HttpGet("courses")]
        public async Task<IActionResult> SearchCourses(
            [FromQuery] string keyword = "",
            [FromQuery] string category = "",
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var courses = await _courseService.SearchCoursesAsync(
                    keyword, category, minPrice, maxPrice, page, pageSize);
                if (!courses.Any())
                    return NotFound(new { message = "Không tìm thấy khóa học nào phù hợp" });

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ✅ Tìm kiếm bài viết
        [HttpGet("posts")]
        public async Task<IActionResult> SearchPosts(
            [FromQuery] string keyword = "",
            [FromQuery] string author = "",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var posts = await _postService.SearchPostsAsync(
                    keyword, author, startDate, endDate, page, pageSize);
                if (!posts.Any())
                    return NotFound(new { message = "Không tìm thấy bài viết nào phù hợp" });

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ✅ Tìm kiếm kết hợp cả khóa học và bài viết
        [HttpGet("all")]
        public async Task<IActionResult> SearchAll(
            [FromQuery] string keyword = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var courses = await _courseService.SearchCoursesAsync(keyword, "", null, null, page, pageSize);
                var posts = await _postService.SearchPostsAsync(keyword, "", null, null, page, pageSize);

                var result = new
                {
                    Courses = courses.Any() ? courses : new List<CourseDto>(),
                    Posts = posts.Any() ? posts : new List<PostDto>()
                };

                if (!courses.Any() && !posts.Any())
                    return NotFound(new { message = "Không tìm thấy kết quả nào phù hợp" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
}