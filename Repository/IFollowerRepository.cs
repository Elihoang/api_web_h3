using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IFollowerRepository
{
    Task<IEnumerable<Follower>> GetAllAsync();
    Task<Follower> GetByIdAsync(Guid id);
    Task<Follower> GetByUsersAsync(Guid followerId, Guid followingId);
    Task<IEnumerable<Follower>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<Follower>> GetFollowingAsync(Guid userId);
    Task AddAsync(Follower follower);
    Task DeleteAsync(Guid id);
}