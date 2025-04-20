using API_WebH3.DTO.Comment;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class CommentService
{
    private readonly ICommentRepository _commentRepository; 
    private readonly IUserRepository _userRepository; 
    private readonly IPostRepository _postRepository;
    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }
    
    
    public async Task<IEnumerable<CommentDto>> GetAllAsync()
    {
        var comments = await _commentRepository.GetCommentAllAsync();
        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            CreatedAt = c.CreatedAt,
            Replies = c.Replies?.Select(r => new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 => new CommentDto
                {
                    Id = r2.Id,
                    UserId = r2.UserId,
                    PostId = r2.PostId,
                    Content = r2.Content,
                    ParentCommentId = r2.ParentCommentId,
                    CreatedAt = r2.CreatedAt,
                    Replies = new List<CommentDto>() 
                }).ToList() ?? new List<CommentDto>()
            }).ToList() ?? new List<CommentDto>()
        });
    }
    
    public async Task<CommentDto> GetByIdAsync(int id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return null;
        }
        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            Replies = comment.Replies?.Select(r => new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 => new CommentDto
                {
                    Id = r2.Id,
                    UserId = r2.UserId,
                    PostId = r2.PostId,
                    Content = r2.Content,
                    ParentCommentId = r2.ParentCommentId,
                    CreatedAt = r2.CreatedAt,
                    Replies = new List<CommentDto>() 
                }).ToList() ?? new List<CommentDto>()
            }).ToList() ?? new List<CommentDto>()
        };
    }
    public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(Guid postId)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId);
        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            CreatedAt = c.CreatedAt,
            Replies = c.Replies?.Select(r => new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 => new CommentDto
                {
                    Id = r2.Id,
                    UserId = r2.UserId,
                    PostId = r2.PostId,
                    Content = r2.Content,
                    ParentCommentId = r2.ParentCommentId,
                    CreatedAt = r2.CreatedAt,
                    Replies = new List<CommentDto>() // Giới hạn độ sâu
                }).ToList() ?? new List<CommentDto>()
            }).ToList() ?? new List<CommentDto>()
        });
    }
    
    public async Task<CommentDto> CreateAsync(CreateCommentDto createCommentDto)
    {
        var user = await _userRepository.GetByIdAsync(createCommentDto.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }


        var post = await _postRepository.GetPostByIdAsync(createCommentDto.PostId);
        if (post == null)
        {
            throw new ArgumentException("Post not found.");
        }


        if (createCommentDto.ParentCommentId.HasValue)
        {
            var parentComment = await _commentRepository.GetCommentByIdAsync(createCommentDto.ParentCommentId.Value);
            if (parentComment == null)
            {
                throw new ArgumentException("Parent comment not found.");
            }
            if (parentComment.PostId != createCommentDto.PostId)
            {
                throw new ArgumentException("Parent comment does not belong to the specified post.");
            }
        }

        var comment = new Comment
        {
            UserId = createCommentDto.UserId,
            PostId = createCommentDto.PostId,
            Content = createCommentDto.Content,
            ParentCommentId = createCommentDto.ParentCommentId,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _commentRepository.AddCommentAsync(comment);

        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            Replies = new List<CommentDto>()
        };
    }
    public async Task<CommentDto> UpdateAsync(int id, UpdateCommentDto updateCommentDto)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return null;
        }

        comment.Content = updateCommentDto.Content;

        await _commentRepository.UpdateCommentAsync(comment);

        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            Replies = comment.Replies?.Select(r => new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 => new CommentDto
                {
                    Id = r2.Id,
                    UserId = r2.UserId,
                    PostId = r2.PostId,
                    Content = r2.Content,
                    ParentCommentId = r2.ParentCommentId,
                    CreatedAt = r2.CreatedAt,
                    Replies = new List<CommentDto>() 
                }).ToList() ?? new List<CommentDto>()
            }).ToList() ?? new List<CommentDto>()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return false;
        }

        await _commentRepository.DeleteCommentAsync(id);
        return true;
    }
}