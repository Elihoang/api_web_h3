using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;


[Route("api/[controller]")]
[ApiController]
public class FilterController : ControllerBase
{
    private readonly FilterService _filterService;

    public FilterController(FilterService filterService)
    {
        _filterService = filterService;
    }

    [HttpGet("courses")]
    public async Task<IActionResult> FilterCourses(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] double? minRating,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        try
        {
            var (courses, total, totalPages) = await _filterService.FilterCoursesAsync(
                category, minPrice, maxPrice, minRating, page, limit);

            if (!courses.Any())
            {
                return NotFound(new { Message = "Không tìm thấy khóa học nào." });
            }

            return Ok(new
            {
                Message = "Lọc khóa học thành công.",
                Data = new
                {
                    Courses = courses,
                    Total = total,
                    Page = page,
                    TotalPages = totalPages
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                Message = "Lỗi khi lọc khóa học.", 
                Error = ex.Message,
                StackTrace = ex.StackTrace // Thêm để debug
            });
        }
    }
}