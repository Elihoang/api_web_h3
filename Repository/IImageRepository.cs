using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IImageRepository
{
    Task<User> GetUserByIdAsync(Guid userId);
    Task<Post> GetPostByIdAsync(Guid postId);
    Task UpdateUserAsync(User user);
    Task UpdatePostAsync(Post post);
}