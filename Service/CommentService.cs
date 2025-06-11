using API_WebH3.DTO.Comment;
using API_WebH3.DTO.User;
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
    return comments.Select(c =>
    {
        var parentComment = c.ParentComment;

        return new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFullName = c.User?.FullName ?? "Unknown",
            UserProfileImage = c.User?.ProfileImage ?? "default-avatar.png",
            PostId = c.PostId,
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            ParentUserFullName = parentComment?.User?.FullName ?? null,
            CreatedAt = c.CreatedAt,
            Replies = c.Replies?.Select(r =>
            {
                var replyParentComment = r.ParentComment;

                return new CommentDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserFullName = r.User?.FullName ?? "Unknown",
                    UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
                    PostId = r.PostId,
                    Content = r.Content,
                    ParentCommentId = r.ParentCommentId,
                    ParentUserFullName = replyParentComment?.User?.FullName ?? null,
                    CreatedAt = r.CreatedAt,
                    Replies = r.Replies?.Select(r2 =>
                    {
                        var replyParentComment2 = r2.ParentComment;

                        return new CommentDto
                        {
                            Id = r2.Id,
                            UserId = r2.UserId,
                            UserFullName = r2.User?.FullName ?? "Unknown",
                            UserProfileImage = r2.User?.ProfileImage ?? "default-avatar.png",
                            PostId = r2.PostId,
                            Content = r2.Content,
                            ParentCommentId = r2.ParentCommentId,
                            ParentUserFullName = replyParentComment2?.User?.FullName ?? null,
                            CreatedAt = r2.CreatedAt,
                            Replies = new List<CommentDto>()
                        };
                    }).ToList() ?? new List<CommentDto>()
                };
            }).ToList() ?? new List<CommentDto>()
        };
    }).ToList();
}
    
    public async Task<CommentDto> GetByIdAsync(int id)
{
    var comment = await _commentRepository.GetCommentByIdAsync(id);
    if (comment == null)
    {
        return null;
    }

    var parentComment = comment.ParentComment;

    return new CommentDto
    {
        Id = comment.Id,
        UserId = comment.UserId,
        UserFullName = comment.User?.FullName ?? "Unknown",
        UserProfileImage = comment.User?.ProfileImage ?? "default-avatar.png",
        PostId = comment.PostId,
        Content = comment.Content,
        ParentCommentId = comment.ParentCommentId,
        ParentUserFullName = parentComment?.User?.FullName ?? null,
        CreatedAt = comment.CreatedAt,
        Replies = comment.Replies?.Select(r =>
        {
            var replyParentComment = r.ParentComment;

            return new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = r.User?.FullName ?? "Unknown",
                UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                ParentUserFullName = replyParentComment?.User?.FullName ?? null,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 =>
                {
                    var replyParentComment2 = r2.ParentComment;

                    return new CommentDto
                    {
                        Id = r2.Id,
                        UserId = r2.UserId,
                        UserFullName = r2.User?.FullName ?? "Unknown",
                        UserProfileImage = r2.User?.ProfileImage ?? "default-avatar.png",
                        PostId = r2.PostId,
                        Content = r2.Content,
                        ParentCommentId = r2.ParentCommentId,
                        ParentUserFullName = replyParentComment2?.User?.FullName ?? null,
                        CreatedAt = r2.CreatedAt,
                        Replies = new List<CommentDto>()
                    };
                }).ToList() ?? new List<CommentDto>()
            };
        }).ToList() ?? new List<CommentDto>()
    };
}
    
    public async Task<CommentDto> CreateAsync(CreateCommentDto createCommentDto)
    {
        var user = await _userRepository.GetByIdAsync(createCommentDto.UserId);
        if (user == null)
        {
            AppLogger.LogError("User not found.");
        }

        var post = await _postRepository.GetPostByIdAsync(createCommentDto.PostId);
        if (post == null)
        {
            AppLogger.LogError("Post not found.");
        }

        // Lấy và kiểm tra bình luận cha trước khi tạo
        Comment? parentComment = null;
        if (createCommentDto.ParentCommentId.HasValue)
        {
            parentComment = await _commentRepository.GetCommentByIdAsync(createCommentDto.ParentCommentId.Value);
            if (parentComment == null)
            {
                AppLogger.LogError("Parent comment not found.");
            }
            if (parentComment.PostId != createCommentDto.PostId)
            {
                AppLogger.LogError("Parent comment does not belong to the specified post.");
            }
        }

        // Tạo bình luận mới
        var comment = new Comment
        {
            UserId = createCommentDto.UserId,
            PostId = createCommentDto.PostId,
            Content = createCommentDto.Content,
            ParentCommentId = createCommentDto.ParentCommentId,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _commentRepository.AddCommentAsync(comment);

        // Trả về CommentDto với parentUserFullName từ parentComment đã tải trước
        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            UserFullName = user.FullName,
            UserProfileImage = user.ProfileImage ?? "default-avatar.png",
            PostId = comment.PostId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            ParentUserFullName = parentComment?.User?.FullName ?? null, 
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
    comment.CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

    await _commentRepository.UpdateCommentAsync(comment);

    var parentComment = comment.ParentComment;

    return new CommentDto
    {
        Id = comment.Id,
        UserId = comment.UserId,
        UserFullName = comment.User?.FullName ?? "Unknown",
        UserProfileImage = comment.User?.ProfileImage ?? "default-avatar.png",
        PostId = comment.PostId,
        Content = comment.Content,
        ParentCommentId = comment.ParentCommentId,
        ParentUserFullName = parentComment?.User?.FullName ?? null,
        CreatedAt = comment.CreatedAt,
        Replies = comment.Replies?.Select(r =>
        {
            var replyParentComment = r.ParentComment;

            return new CommentDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = r.User?.FullName ?? "Unknown",
                UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
                PostId = r.PostId,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                ParentUserFullName = replyParentComment?.User?.FullName ?? null,
                CreatedAt = r.CreatedAt,
                Replies = r.Replies?.Select(r2 =>
                {
                    var replyParentComment2 = r2.ParentComment;

                    return new CommentDto
                    {
                        Id = r2.Id,
                        UserId = r2.UserId,
                        UserFullName = r2.User?.FullName ?? "Unknown",
                        UserProfileImage = r2.User?.ProfileImage ?? "default-avatar.png",
                        PostId = r2.PostId,
                        Content = r2.Content,
                        ParentCommentId = r2.ParentCommentId,
                        ParentUserFullName = replyParentComment2?.User?.FullName ?? null,
                        CreatedAt = r2.CreatedAt,
                        Replies = new List<CommentDto>()
                    };
                }).ToList() ?? new List<CommentDto>()
            };
        }).ToList() ?? new List<CommentDto>()
    };
}

    // Hàm lấy tất cả ID của phản hồi lồng nhau
    private async Task<List<int>> GetAllReplyIdsAsync(int commentId)
    {
        var replyIds = new List<int>();
        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        
        if (comment == null || comment.Replies == null)
        {
            return replyIds;
        }

        foreach (var reply in comment.Replies)
        {
            replyIds.Add(reply.Id);
            replyIds.AddRange(await GetAllReplyIdsAsync(reply.Id));
        }

        return replyIds;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return false;
        }

        // Lấy danh sách tất cả ID của phản hồi lồng nhau
        var replyIds = await GetAllReplyIdsAsync(id);

        // Xóa tất cả phản hồi
        foreach (var replyId in replyIds)
        {
            await _commentRepository.DeleteCommentAsync(replyId);
        }

        // Xóa bình luận mẹ
        await _commentRepository.DeleteCommentAsync(id);

        return true;
    }
    
    public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(Guid postId)
{
    var comments = await _commentRepository.GetByPostIdAsync(postId);

    return comments.Select(c =>
    {
        // Sử dụng c.ParentComment đã được tải từ repository
        var parentComment = c.ParentComment;
        if (c.ParentComment == null && c.ParentCommentId.HasValue)
        {
            Console.WriteLine($"ParentComment not loaded for comment {c.Id}, ParentCommentId: {c.ParentCommentId}");
        }
        if (c.ParentComment?.User == null)
        {
            Console.WriteLine($"User not loaded for ParentComment of comment {c.Id}");
        }
        return new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFullName = c.User?.FullName ?? "Unknown",
            UserProfileImage = c.User?.ProfileImage ?? "default-avatar.png",
            PostId = c.PostId,
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            ParentUserFullName = parentComment?.User?.FullName ?? null, // Lấy FullName từ ParentComment.User
            CreatedAt = c.CreatedAt,
            Replies = c.Replies?.Select(r =>
            {
                var replyParentComment = r.ParentComment;

                return new CommentDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserFullName = r.User?.FullName ?? "Unknown",
                    UserProfileImage = r.User?.ProfileImage ?? "default-avatar.png",
                    PostId = r.PostId,
                    Content = r.Content,
                    ParentCommentId = r.ParentCommentId,
                    ParentUserFullName = replyParentComment?.User?.FullName ?? null,
                    CreatedAt = r.CreatedAt,
                    Replies = r.Replies?.Select(r2 =>
                    {
                        var replyParentComment2 = r2.ParentComment;

                        return new CommentDto
                        {
                            Id = r2.Id,
                            UserId = r2.UserId,
                            UserFullName = r2.User?.FullName ?? "Unknown",
                            UserProfileImage = r2.User?.ProfileImage ?? "default-avatar.png",
                            PostId = r2.PostId,
                            Content = r2.Content,
                            ParentCommentId = r2.ParentCommentId,
                            ParentUserFullName = replyParentComment2?.User?.FullName ?? null,
                            CreatedAt = r2.CreatedAt,
                            Replies = new List<CommentDto>()
                        };
                    }).ToList() ?? new List<CommentDto>()
                };
            }).ToList() ?? new List<CommentDto>()
        };
    }).ToList();
}
}