using API_WebH3.DTOs.Review;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviews()
    {
        var reviews = await _reviewRepository.GetAllReviewsAsync();
        return reviews.Select(r=>new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            CourseId = r.CourseId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
        }).ToList();
    }

    public async Task<ReviewDto> GetReviewById(int id)
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
            CreatedAt = review.CreatedAt,
        };
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewByUserId(Guid userId)
    {
        var reviews = await _reviewRepository.GetByUserIdAsync(userId);
        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            CourseId = r.CourseId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
        }).ToList();
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseId(Guid courseId)
    {
        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);
        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            CourseId = r.CourseId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
    public async Task<ReviewDto> CreateReview(CreateReviewDto createReviewDto)
    {
        var reviews = new Review
        {
            UserId = createReviewDto.UserId,
            CourseId = createReviewDto.CourseId,
            Rating = createReviewDto.Rating,
            Comment = createReviewDto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        await _reviewRepository.CreateReviewAsync(reviews);

        return new ReviewDto
        {
            Id = reviews.Id,
            UserId = reviews.UserId,
            CourseId = reviews.CourseId,
            Rating = reviews.Rating,
            Comment = reviews.Comment,
            CreatedAt = reviews.CreatedAt
        };
    }

    public async Task<ReviewDto> UpdateReview(int id, UpdateReviewDto updateReviewDto)
    {
        var reviews = await _reviewRepository.GetReviewByIdAsync(id);
        if (reviews == null)
        {
            return null;
        }
        
        reviews.Rating = updateReviewDto.Rating;
        reviews.Comment = updateReviewDto.Comment;
        reviews.CreatedAt = DateTime.Now;
         var updateReview = await _reviewRepository.UpdateReviewAsync(reviews);
         return new ReviewDto
         {
             Id = updateReview.Id,
             UserId = updateReview.UserId,
             CourseId = updateReview.CourseId,
             Rating = updateReview.Rating,
             Comment = updateReview.Comment,
             CreatedAt = updateReview.CreatedAt
         };
    }

    public async Task<bool> DeleteReview(int id)
    {
        return await _reviewRepository.DeleteReviewAsync(id);
    }
}