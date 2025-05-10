using API_WebH3.Models;
using System;

namespace API_WebH3.Repository;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllReviewAsync();
    Task<Review> GetReviewByIdAsync(int id);
    Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(string courseId);
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
    Task AddReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
}