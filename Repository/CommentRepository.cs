using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_WebH3.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Comment> GetCommentByIdAsync(int id)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.ParentComment)
            .ThenInclude(pc => pc.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.Replies)
            .ThenInclude(r2 => r2.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Comment>> GetCommentAllAsync()
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId)
    {
        return await _context.Comments
            .Include(c => c.User) // Lấy thông tin người dùng
            .Include(c => c.ParentComment) // Lấy bình luận cha
            .ThenInclude(pc => pc.User) // Lấy thông tin người dùng của bình luận cha
            .Include(c => c.Replies) // Lấy các bình luận trả lời
            .ThenInclude(r => r.User) // Lấy thông tin người dùng của trả lời
            .Include(c => c.Replies) // Lấy trả lời cấp 1
            .ThenInclude(r => r.Replies) // Lấy trả lời cấp 2
            .ThenInclude(r2 => r2.User) // Lấy thông tin người dùng của trả lời cấp 2
            .Where(c => c.PostId == postId)
            .ToListAsync();
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
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}