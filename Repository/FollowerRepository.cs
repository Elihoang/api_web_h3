using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class FollowerRepository : IFollowerRepository
{
    private readonly AppDbContext _context;

    public FollowerRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Follower>> GetAllAsync()
    {
        return await _context.Followers.ToListAsync();
    }

    public async Task<Follower> GetByIdAsync(Guid id)
    {
        return await _context.Followers.FindAsync(id);
    }

    public async Task<Follower> GetByUsersAsync(Guid followerId, Guid followingId)
    {
        return await _context.Followers
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task<IEnumerable<Follower>> GetFollowersAsync(Guid userId)
    {
        return await _context.Followers
            .Where(f => f.FollowingId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Follower>> GetFollowingAsync(Guid userId)
    {
        return await _context.Followers
            .Where(f => f.FollowerId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Follower follower)
    {
        await _context.Followers.AddAsync(follower);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var follower = await _context.Followers.FindAsync(id);
        if (follower != null)
        {
            _context.Followers.Remove(follower);
            await _context.SaveChangesAsync();
        }
    }
}