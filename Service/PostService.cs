using API_WebH3.DTO.Post;
using API_WebH3.DTO.User;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    
    public PostService(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetPostAllAsync();
        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Title = p.Title,
            Content = p.Content,
            Tags = p.Tags,
            UrlImage = p.UrlImage,
            CreatedAt = p.CreatedAt
        });
    }
    public async Task<PostDto> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return null;
        }
        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            Tags = post.Tags,
            UrlImage = post.UrlImage,
            CreatedAt = post.CreatedAt
        };
    }
    public async Task<PostDto> CreateAsync(CreatePostDto createPostDto)
    {
        var user = await _userRepository.GetByIdAsync(createPostDto.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = createPostDto.UserId,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            Tags = createPostDto.Tags,
            UrlImage = createPostDto.UrlImage,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _postRepository.AddPostAsync(post);

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            Tags = post.Tags,
            UrlImage = post.UrlImage,
            CreatedAt = post.CreatedAt
        };
    }
    public async Task<PostDto> UpdateAsync(Guid id, UpdatePostDto updatePostDto)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return null;
        }

        post.Title = updatePostDto.Title;
        post.Content = updatePostDto.Content;
        post.Tags = updatePostDto.Tags;
        post.UrlImage = updatePostDto.UrlImage;

        await _postRepository.UpdatePostAsync(post);

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            Tags = post.Tags,
            UrlImage = post.UrlImage,
            CreatedAt = post.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return false;
        }

        await _postRepository.DeletePostAsync(id);
        return true;
    }
    public async Task<IEnumerable<PostDto>> SearchPostsAsync(
        string keyword,
        string author,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize)
    {

        var posts = await _postRepository.SearchPostsAsync(keyword, page, pageSize);
        
        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            CreatedAt = p.CreatedAt,
            Tags = p.Tags,
            UrlImage = p.UrlImage,
            User= new UserDto
            {
                Id = p.User.Id,
                FullName = p.User.FullName,
                Email = p.User.Email,
                ProfileImage = p.User?.ProfileImage,
                Role = p.User.Role,
                BirthDate = p.User.BirthDate,
                CreatedAt = p.User.CreatedAt
            }
        }).ToList();
    }
}