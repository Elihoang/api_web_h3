using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllReviewAsync();
    Task<Review> GetReviewByIdAsync(int id);
    Task AddReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
}