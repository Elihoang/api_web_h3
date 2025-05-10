using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_WebH3.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllReviewAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task<Review> GetReviewByIdAsync(int id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(string courseId)
    {
        if (string.IsNullOrEmpty(courseId))
        {
            throw new ArgumentException("CourseId cannot be null or empty.", nameof(courseId));
        }
        return await _context.Reviews.Where(r => r.CourseId == courseId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task AddReviewAsync(Review review)
    {
        if (review == null)
        {
            throw new ArgumentNullException(nameof(review));
        }
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(Review review)
    {
        if (review == null)
        {
            throw new ArgumentNullException(nameof(review));
        }
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}