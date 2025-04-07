using API_WebH3.DTOs.Course;
using API_WebH3.DTOs.Post;
using API_WebH3.DTOs.User;
using API_WebH3.Models;
using API_WebH3.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Services
{
    public class PostService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }
        
        public async Task<IEnumerable<PostDto>> SearchPostsAsync(
            string keyword,
            string author,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize)
        {
            // Gọi phương thức tìm kiếm từ repository
            var posts = await _postRepository.SearchPostsAsync(keyword, page, pageSize);

            // Ánh xạ sang DTO
            return posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Tags = p.Tags,
                UrlImage = p.UrlImage,
                User = new UserDTO
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

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _postRepository.GetAllPostsAsync(); 

            return posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Tags = p.Tags,
                UrlImage = p.UrlImage,
                User =  new UserDTO
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

        public async Task<PostDto> AddPostAsync(CreatePostDto postDto)
        {
            
            var userExists = await _userRepository.ExistsAsync(postDto.UserId);
            if (!userExists)
            {
                throw new Exception("❌ UserId không hợp lệ: Người dùng không tồn tại!");
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = postDto.Title,
                Content = postDto.Content,
                UserId = postDto.UserId,
                Tags = postDto.Tags,
            };

            await _postRepository.AddPostAsync(post);
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Tags = post.Tags,
                UrlImage = post.UrlImage,
            };  
        }


        public async Task<PostDto?> GetPostByIdAsync(Guid id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null) return null;

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Tags = post.Tags,
                UrlImage = post.UrlImage,
                User = new UserDTO
                {
                    Id = post.User.Id,
                    FullName = post.User.FullName,
                    Email = post.User.Email,
                    ProfileImage = post.User?.ProfileImage,
                    Role = post.User.Role,
                    BirthDate = post.User.BirthDate,
                    CreatedAt = post.User.CreatedAt
                }
            };
        }

        public async Task DeletePostAsync(Guid id)
        {        
            await _postRepository.DeletePostAsync(id);
        }

        public async Task UpdatePostAsync(Guid id, CreatePostDto postDto)
        {
            var existingPost = await _postRepository.GetPostByIdAsync(id);
            if (existingPost == null) throw new Exception("UPDATE: Không tìm thấy khóa học!");

            existingPost.Title = postDto.Title;
            existingPost.Content = postDto.Content;
            existingPost.Tags = postDto.Tags;
            await _postRepository.UpdatePostAsync(existingPost);
        }

        public async Task<string?> UploadImageAsync(Guid id, IFormFile file)
        {
            return await _postRepository.UploadImageAsync(id, file);
        }
    }
}
