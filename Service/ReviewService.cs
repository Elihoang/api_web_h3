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
        var reviewDtos = new List<ReviewDto>();

        foreach (var r in reviews)
        {
            var user = await _userRepository.GetByIdAsync(r.UserId); // Lấy thông tin User
            reviewDtos.Add(new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                CourseId = r.CourseId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UserFullName = user?.FullName ?? string.Empty, // Gán FullName, xử lý null
                UserProfileImage = user?.ProfileImage ?? string.Empty // Gán ProfileImage, xử lý null
            });
        }

        return reviewDtos;
    }

    public async Task<ReviewDto> GetByIdAsync(int id)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(id);
        if (review == null)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(review.UserId); // Lấy thông tin User
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            CourseId = review.CourseId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UserFullName = user?.FullName ?? string.Empty, // Gán FullName, xử lý null
            UserProfileImage = user?.ProfileImage ?? string.Empty // Gán ProfileImage, xử lý null
        };
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(string courseId)
    {
        if (string.IsNullOrEmpty(courseId))
        {
            AppLogger.LogError($"CourseId cannot be null or empty.");
            throw new ArgumentException("CourseId cannot be null or empty.", nameof(courseId));
        }
        var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);
        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            CourseId = r.CourseId,
            UserFullName = r.User?.FullName ?? "Unknown",
            UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            AppLogger.LogError($"UserId cannot be empty.");
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }
        var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);
        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserFullName = r.User?.FullName ?? "Unknown",
            UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
            CourseId = r.CourseId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto createReviewDto)
    {
        var user = await _userRepository.GetByIdAsync(createReviewDto.UserId);
        if (user == null)
        {
            AppLogger.LogError("User not found.");
            throw new ArgumentException("User not found.");
        }

        var course = await _courseRepository.GetByIdAsync(createReviewDto.CourseId);
        if (course == null)
        {
            AppLogger.LogError("Course not found.");
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
            CreatedAt = review.CreatedAt,
            UserFullName = user.FullName, // Gán FullName từ User
            UserProfileImage = user.ProfileImage ?? string.Empty // Gán ProfileImage, xử lý null
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

        var user = await _userRepository.GetByIdAsync(review.UserId); // Lấy thông tin User
        AppLogger.LogInfo($"Tên người đánh giá: {review.User.FullName}");
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            CourseId = review.CourseId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UserFullName = review.User.FullName ?? string.Empty, // Gán FullName, xử lý null
            UserProfileImage = user?.ProfileImage ?? string.Empty // Gán ProfileImage, xử lý null
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