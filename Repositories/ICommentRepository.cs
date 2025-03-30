using API_WebH3.Models;

namespace API_WebH3.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetAllCommentsAsync();
    Task<Comment> GetCommentByIdAsync(int id);
    Task<IEnumerable<Comment>> GetCommentByUserIdAsync(Guid userId);
    Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<Comment> UpdateCommentAsync(Comment comment); 
    Task <bool> DeleteCommentAsync(int id);
}