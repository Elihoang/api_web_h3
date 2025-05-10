using API_WebH3.DTO.Review;
using API_WebH3.Models;
using API_WebH3.Repository;
using System;

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

    public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(string courseId)
    {
        if (string.IsNullOrEmpty(courseId))
        {
            throw new ArgumentException("CourseId cannot be null or empty.", nameof(courseId));
        }
        var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);
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

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }
        var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);
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

    public async Task<ReviewDto> CreateAsync(CreateReviewDto createReviewDto)
    {
        if (createReviewDto == null)
        {
            throw new ArgumentNullException(nameof(createReviewDto));
        }

       
        // Check if user has already reviewed this course
        var existingReviews = await _reviewRepository.GetReviewsByUserIdAsync(createReviewDto.UserId);
        if (existingReviews.Any(r => r.CourseId == createReviewDto.CourseId))
        {
            throw new InvalidOperationException("User has already submitted a review for this course.");
        }

        var user = await _userRepository.GetByIdAsync(createReviewDto.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found.", nameof(createReviewDto.UserId));
        }

        var course = await _courseRepository.GetByIdAsync(createReviewDto.CourseId);
        if (course == null)
        {
            throw new ArgumentException("Course not found.", nameof(createReviewDto.CourseId));
        }

        var review = new Review
        {
            UserId = createReviewDto.UserId,
            CourseId = createReviewDto.CourseId,
            Rating = createReviewDto.Rating,
            Comment = createReviewDto.Comment ?? string.Empty,
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
        if (updateReviewDto == null)
        {
            throw new ArgumentNullException(nameof(updateReviewDto));
        }

        var review = await _reviewRepository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return null;
        }

        review.Rating = updateReviewDto.Rating;
        review.Comment = updateReviewDto.Comment ?? string.Empty;

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