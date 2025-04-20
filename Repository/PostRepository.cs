using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class PostRepository : IPostRepository
{
    
    private readonly AppDbContext _context;
    
    public PostRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Post>> GetPostAllAsync()
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<Post> GetPostByIdAsync(Guid id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await GetPostByIdAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}