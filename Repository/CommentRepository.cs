using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class CommentRepository : ICommentRepository
{
    
    private readonly AppDbContext _context;
    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<IEnumerable<Comment>> GetCommentAllAsync()
    {
        return await _context.Comments
            .Include(c => c.Replies)
            .ToListAsync();
    }

    public async Task<Comment> GetCommentByIdAsync(int id)
    {
        return await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment.Replies.Any())
        {
            _context.Comments.RemoveRange(comment.Replies);
        }
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId)
    {
        return await _context.Comments
            .Include(c => c.Replies)
            .Where(c => c.PostId == postId)
            .ToListAsync();
    }
}