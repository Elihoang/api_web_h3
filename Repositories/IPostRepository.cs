using API_WebH3.Models;

namespace API_WebH3.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post?> GetPostByIdAsync(Guid id);
        Task AddPostAsync(Post post);
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(Guid id);
        Task<string?> UploadImageAsync(Guid Id, IFormFile file);
        Task<IEnumerable<Post>> SearchPostsAsync(string keyword, int page, int pageSize);
    }
}
