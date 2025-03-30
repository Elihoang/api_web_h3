using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllReviewsAsync();
    Task<Review> GetReviewByIdAsync(int id);
    Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
    Task<Review> CreateReviewAsync(Review review);
    Task<Review> UpdateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int id);
}