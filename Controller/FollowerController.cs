using API_WebH3.DTO.Follower;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;


[ApiController]
[Route("api/[controller]")]
public class FollowerController : ControllerBase
{
    private readonly FollowerService _followerService;
    public FollowerController(FollowerService followerService)
    {
        _followerService = followerService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FollowerDto>>> GetFollowers()
    {
        var followers = await _followerService.GetAllAsync();
        return Ok(followers);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<FollowerDto>> GetFollower(Guid id)
    {
        var follower = await _followerService.GetByIdAsync(id);
        if (follower == null)
        {
            return NotFound();
        }
        return Ok(follower);
    }
    [HttpGet("followers/{userId}")]
    public async Task<ActionResult<IEnumerable<FollowerDto>>> GetFollowersByUser(Guid userId)
    {
        var followers = await _followerService.GetFollowersAsync(userId);
        return Ok(followers);
    }

    [HttpGet("following/{userId}")]
    public async Task<ActionResult<IEnumerable<FollowerDto>>> GetFollowingByUser(Guid userId)
    {
        var following = await _followerService.GetFollowingAsync(userId);
        return Ok(following);
    }
    [HttpPost]
    public async Task<ActionResult<FollowerDto>> CreateFollower(CreateFollowerDto createFollowerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var followerDto = await _followerService.CreateAsync(createFollowerDto);
        return CreatedAtAction(nameof(GetFollower), new { id = followerDto.Id }, followerDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFollower(Guid id)
    {
        var result = await _followerService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}