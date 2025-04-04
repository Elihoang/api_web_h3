using API_WebH3.DTOs.Review;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    
    private readonly ReviewService _reviewService;

    public ReviewController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviewsAsync()
    {
        var reviews = await _reviewService.GetAllReviews();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReviewByIdAsync(int id)
    {
        var review = await _reviewService.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }
        
        return Ok(review);
    }

    [HttpGet("Course/{courseId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByCourseAsync(Guid courseId)
    {
        var review = await _reviewService.GetReviewsByCourseId(courseId);
        if (review == null)
        {
            return NotFound();
        }
        return Ok(review);
    }

    [HttpGet("User/{userId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUserAsync(Guid userId)
    {
        var review = await _reviewService.GetReviewByUserId(userId);
        if (review == null)
        {
            return NotFound();
        }
        return Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReviewAsync([FromBody] CreateReviewDto createReviewDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var review = await _reviewService.CreateReview(createReviewDto);
            return CreatedAtAction("GetReviewById", new { id = review.Id }, review);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // Trả về lỗi 409 Conflict
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewDto>> UpdateReviewAsync(int id, [FromBody] UpdateReviewDto updateReviewDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var review = await _reviewService.UpdateReview(id, updateReviewDto);
        if (review == null)
        {
            return NotFound();
        }
        
        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ReviewDto>> DeleteReviewAsync(int id)
    {
        var review = await _reviewService.DeleteReview(id);
        if (review == null)
        {
            return NotFound();
        }

        return Ok();
    }
}