using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentAllAsync();
    Task<Comment> GetCommentByIdAsync(int id);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(int id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId);
}