using System.Net.Http.Headers;
using API_WebH3.DTO.Follower;
using API_WebH3.Models;
using API_WebH3.Repository;

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
        return followers.Select(f => new FollowerDto
        {
            Id = f.Id,
            FollowerId = f.FollowerId,
            FollowingId = f.FollowingId,
            CreatedAt = f.CreatedAt
        });
    }

    public async Task<FollowerDto> GetByIdAsync(Guid id)
    {
        var follower = await _followerRepository.GetByIdAsync(id);
        if (follower == null)
        {
            return null;
        }
        return new FollowerDto
        {
            Id = follower.Id,
            FollowerId = follower.FollowerId,
            FollowingId = follower.FollowingId,
            CreatedAt = follower.CreatedAt
        };
    }
    
    public async Task<IEnumerable<FollowerDto>> GetFollowersAsync(Guid userId)
    {
        var followers = await _followerRepository.GetFollowersAsync(userId);
        return followers.Select(f => new FollowerDto
        {
            Id = f.Id,
            FollowerId = f.FollowerId,
            FollowingId = f.FollowingId,
            CreatedAt = f.CreatedAt
        });
    }

    public async Task<IEnumerable<FollowerDto>> GetFollowingAsync(Guid userId)
    {
        var following = await _followerRepository.GetFollowingAsync(userId);
        return following.Select(f => new FollowerDto
        {
            Id = f.Id,
            FollowerId = f.FollowerId,
            FollowingId = f.FollowingId,
            CreatedAt = f.CreatedAt
        });
    }
    public async Task<FollowerDto> CreateAsync(CreateFollowerDto createFollowerDto)
    {
        var followerUser = await _userRepository.GetByIdAsync(createFollowerDto.FollowerId);
        if (followerUser == null)
        {
            throw new ArgumentException("Follower user not found.");
        }

        var followingUser = await _userRepository.GetByIdAsync(createFollowerDto.FollowingId);
        if (followingUser == null)
        {
            throw new ArgumentException("Following user not found.");
        }

        if (createFollowerDto.FollowerId == createFollowerDto.FollowingId)
        {
            throw new ArgumentException("A user cannot follow themselves.");
        }

        var existingFollow = await _followerRepository.GetByUsersAsync(createFollowerDto.FollowerId, createFollowerDto.FollowingId);
        if (existingFollow != null)
        {
            throw new ArgumentException("This follow relationship already exists.");
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
            FollowingId = follower.FollowingId,
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