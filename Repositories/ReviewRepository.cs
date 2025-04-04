using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class ReviewRepository : IReviewRepository
{
    
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Review>> GetAllReviewsAsync()
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course).ToListAsync();
    }

    public async Task<Review> GetReviewByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Reviews.Where(r => r.CourseId == courseId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
         _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
        
    }

    public async  Task<Review> UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        return review;
        
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return false;
        }
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<Review?> GetReviewByUserIdAndCourseId(Guid userId, Guid courseId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);
    }

   
}