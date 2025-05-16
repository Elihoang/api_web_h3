using System.Net.Http.Headers;
using API_WebH3.DTO.Follower;
using API_WebH3.Models;
using API_WebH3.Repository;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Service;

public class FollowerService
{
    private readonly IFollowerRepository _followerRepository;
    private readonly IUserRepository _userRepository;

    public FollowerService(IFollowerRepository followerRepository, IUserRepository userRepository)
    {
        _followerRepository = followerRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<FollowerDto>> GetAllAsync()
    {
        var followers = await _followerRepository.GetAllAsync();
        var followerDtos = new List<FollowerDto>();

        foreach (var f in followers)
        {
            var followerUser = await _userRepository.GetByIdAsync(f.FollowerId);
            var followingUser = await _userRepository.GetByIdAsync(f.FollowingId);

            followerDtos.Add(new FollowerDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowerFullName = followerUser?.FullName ?? "Không có tên",
                FollowerEmail = followerUser?.Email ?? "Chưa có email",
                FollowingId = f.FollowingId,
                FollowingFullName = followingUser?.FullName ?? "Không có tên",
                FollowingEmail = followingUser?.Email ?? "Chưa có email",
                CreatedAt = f.CreatedAt
            });
        }

        return followerDtos;
    }

    public async Task<FollowerDto> GetByIdAsync(Guid id)
    {
        var follower = await _followerRepository.GetByIdAsync(id);
        if (follower == null)
        {
            return null;
        }

        var followerUser = await _userRepository.GetByIdAsync(follower.FollowerId);
        var followingUser = await _userRepository.GetByIdAsync(follower.FollowingId);

        return new FollowerDto
        {
            Id = follower.Id,
            FollowerId = follower.FollowerId,
            FollowerFullName = followerUser?.FullName ?? "Không có tên",
            FollowerEmail = followerUser?.Email ?? "Chưa có email",
            FollowingId = follower.FollowingId,
            FollowingFullName = followingUser?.FullName ?? "Không có tên",
            FollowingEmail = followingUser?.Email ?? "Chưa có email",
            CreatedAt = follower.CreatedAt
        };
    }
    
    public async Task<IEnumerable<FollowerDto>> GetFollowersAsync(Guid userId)
    {
        var followers = await _followerRepository.GetFollowersAsync(userId);
        var followerDtos = new List<FollowerDto>();

        foreach (var f in followers)
        {
            var followerUser = await _userRepository.GetByIdAsync(f.FollowerId);
            var followingUser = await _userRepository.GetByIdAsync(f.FollowingId);

            followerDtos.Add(new FollowerDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowerFullName = followerUser?.FullName ?? "Không có tên",
                FollowerEmail = followerUser?.Email ?? "Chưa có email",
                FollowingId = f.FollowingId,
                FollowingFullName = followingUser?.FullName ?? "Không có tên",
                FollowingEmail = followingUser?.Email ?? "Chưa có email",
                CreatedAt = f.CreatedAt
            });
        }

        return followerDtos;
    }

    public async Task<IEnumerable<FollowerDto>> GetFollowingAsync(Guid userId)
    {
        var following = await _followerRepository.GetFollowingAsync(userId);
        var followerDtos = new List<FollowerDto>();

        foreach (var f in following)
        {
            var followerUser = await _userRepository.GetByIdAsync(f.FollowerId);
            var followingUser = await _userRepository.GetByIdAsync(f.FollowingId);

            followerDtos.Add(new FollowerDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowerFullName = followerUser?.FullName ?? "Không có tên",
                FollowerEmail = followerUser?.Email ?? "Chưa có email",
                FollowingId = f.FollowingId,
                FollowingFullName = followingUser?.FullName ?? "Không có tên",
                FollowingEmail = followingUser?.Email ?? "Chưa có email",
                CreatedAt = f.CreatedAt
            });
        }

        return followerDtos;
    }

    public async Task<FollowerDto> CreateAsync(CreateFollowerDto createFollowerDto)
    {
        var followerUser = await _userRepository.GetByIdAsync(createFollowerDto.FollowerId);
        if (followerUser == null)
        {
            throw new ArgumentException("Người theo dõi không tồn tại.");
        }

        var followingUser = await _userRepository.GetByIdAsync(createFollowerDto.FollowingId);
        if (followingUser == null)
        {
            throw new ArgumentException("Người được theo dõi không tồn tại.");
        }

        if (createFollowerDto.FollowerId == createFollowerDto.FollowingId)
        {
            throw new ArgumentException("Người dùng không thể tự theo dõi chính Ascendants | 21:44 Không thể tự theo dõi chính mình");
        }

        var existingFollow = await _followerRepository.GetByUsersAsync(createFollowerDto.FollowerId, createFollowerDto.FollowingId);
        if (existingFollow != null)
        {
            throw new ArgumentException("Mối quan hệ theo dõi này đã tồn tại.");
        }

        var follower = new Follower
        {
            Id = Guid.NewGuid(),
            FollowerId = createFollowerDto.FollowerId,
            FollowingId = createFollowerDto.FollowingId,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _followerRepository.AddAsync(follower);

        return new FollowerDto
        {
            Id = follower.Id,
            FollowerId = follower.FollowerId,
            FollowerFullName = followerUser.FullName ?? "Không có tên",
            FollowerEmail = followerUser.Email ?? "Chưa có email",
            FollowingId = follower.FollowingId,
            FollowingFullName = followingUser.FullName ?? "Không có tên",
            FollowingEmail = followingUser.Email ?? "Chưa có email",
            CreatedAt = follower.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var follower = await _followerRepository.GetByIdAsync(id);
        if (follower == null)
        {
            return false;
        }

        await _followerRepository.DeleteAsync(id);
        return true;
    }
}