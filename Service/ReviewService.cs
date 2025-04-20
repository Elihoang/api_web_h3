using API_WebH3.DTO.Review;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    
    public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }
    public async Task<IEnumerable<ReviewDto>> GetAllAsync()
{
    var reviews = await _reviewRepository.GetAllReviewAsync();
    return reviews.Select(r => new ReviewDto
    {
        Id = r.Id,
        UserId = r.UserId,
        CourseId = r.CourseId,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt
    });
}

public async Task<ReviewDto> GetByIdAsync(int id)
{
    var review = await _reviewRepository.GetReviewByIdAsync(id);
    if (review == null)
    {
        return null;
    }
    return new ReviewDto
    {
        Id = review.Id,
        UserId = review.UserId,
        CourseId = review.CourseId,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt
    };
}

public async Task<ReviewDto> CreateAsync(CreateReviewDto createReviewDto)
{
    var user = await _userRepository.GetByIdAsync(createReviewDto.UserId);
    if (user == null)
    {
        throw new ArgumentException("User not found.");
    }

    var course = await _courseRepository.GetByIdAsync(createReviewDto.CourseId);
    if (course == null)
    {
        throw new ArgumentException("Course not found.");
    }

    var review = new Review
    {
        UserId = createReviewDto.UserId,
        CourseId = createReviewDto.CourseId,
        Rating = createReviewDto.Rating,
        Comment = createReviewDto.Comment,
        CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
    };

    await _reviewRepository.AddReviewAsync(review);

    return new ReviewDto
    {
        Id = review.Id,
        UserId = review.UserId,
        CourseId = review.CourseId,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt
    };
}

public async Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto updateReviewDto)
{
    var review = await _reviewRepository.GetReviewByIdAsync(id);
    if (review == null)
    {
        return null;
    }

    // Cập nhật các trường từ DTO
    review.Rating = updateReviewDto.Rating;
    review.Comment = updateReviewDto.Comment;

    await _reviewRepository.UpdateReviewAsync(review);

    return new ReviewDto
    {
        Id = review.Id,
        UserId = review.UserId,
        CourseId = review.CourseId,
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt
    };
}

    public async Task<bool> DeleteAsync(int id)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return false;
        }
        await _reviewRepository.DeleteReviewAsync(id);
        return true;
        
    }
}