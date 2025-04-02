using API_WebH3.DTOs.Course;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // ✅ Lấy tất cả khóa học
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // ✅ Lấy khóa học theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        // ✅ Tạo mới khóa học
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto courseDto)
        {
            if (courseDto == null)
                return BadRequest(new { message = "Invalid course data" });

            var createdCourse = await _courseService.AddCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourseById), new { id = createdCourse.Id }, createdCourse);
        }

        // ✅ Cập nhật khóa học
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CreateCourseDto courseDto)
        {
            if (courseDto == null)
                return BadRequest(new { message = "Invalid course data" });

            var existingCourse = await _courseService.GetCourseByIdAsync(id);
            if (existingCourse == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            await _courseService.UpdateCourseAsync(id, courseDto);
            return Ok(new { message = "Cập nhập khóa học thành công!" });
        }

        // ✅ Xóa khóa học
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học!" });

            await _courseService.DeleteCourseAsync(id);
            return Ok(new { message = "Xóa khóa học thành công!" });
        }

        [HttpPost("upload-image/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(string id, IFormFile file)
        {
            var imageUrl = await _courseService.UploadImageAsync(Guid.Parse(id), file);
            if (imageUrl == null)
                return BadRequest("Upload failed or Course not found.");

            return Ok(new { message = "Image uploaded successfully!", imageUrl });
        }
    }
}
