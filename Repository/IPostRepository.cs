using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostAllAsync();
    Task<Post> GetPostByIdAsync(Guid id);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(Guid id);
}