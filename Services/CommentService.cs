using API_WebH3.DTOs.Comment;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class CommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
       _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentDto>> GetAllComments()
    {
        var comment = await _commentRepository.GetAllCommentsAsync();
        return comment.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
        }).ToList();
    }

    public async Task<CommentDto> GetCommentById(int id)
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
            CreatedAt = comment.CreatedAt,
        };
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByUserId(Guid userId)
    {
        var comment = await _commentRepository.GetCommentByUserIdAsync(userId);
        return comment.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostId(Guid postId)
    {
        var comment = await _commentRepository.GetCommentsByPostAsync(postId);
        return comment.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<CommentDto> CreateComment(CreateCommentDto createCommentDto)
    {

        var comments = new Comment
        {
            UserId = createCommentDto.UserId,
            PostId = createCommentDto.PostId,
            Content = createCommentDto.Content
        };
        await _commentRepository.CreateCommentAsync(comments);

        return new CommentDto
        {
            Id = comments.Id,
            UserId = comments.UserId,
            PostId = comments.PostId,
            Content = comments.Content,
            CreatedAt = comments.CreatedAt
        };
    }

    public async Task<CommentDto> UpdateComment(int id, UpdateCommentDto updateCommentDto)
    {
        var comments = await _commentRepository.GetCommentByIdAsync(id);

        if (comments == null)
        {
            return null;
        }
        
        comments.Content = updateCommentDto.Content;
        
        var updatedComment = await _commentRepository.UpdateCommentAsync(comments);

        return new CommentDto
        {
            Id = updatedComment.Id,
            UserId = updatedComment.UserId,
            PostId = updatedComment.PostId,
            Content = updatedComment.Content,
            CreatedAt = updatedComment.CreatedAt
        };
    }

    public async Task<bool> DeleteComment(int id)
    {
        return await _commentRepository.DeleteCommentAsync(id);
    }
    
}