using System.Security.Claims;
using API_WebH3.DTO.User;
using API_WebH3.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly PhotoService _photoService;

    public UserController(UserService userService, PhotoService photoService)
    {
        _userService = userService;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userDto = await _userService.CreateAsync(createUserDto);
        return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userDto = await _userService.UpdateAsync(id, updateUserDto);
        if (userDto == null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> UpdatePassword(Guid id, [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.UpdatePasswordAsync(id, updatePasswordDto);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id}/upload-profile-image")]
    [Authorize]
    public async Task<IActionResult> UploadProfileImage(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Không có tệp nào được tải lên.");

        // Tải ảnh lên Cloudinary
        var imageUrl = await _photoService.UploadImageAsync(file);
        if (imageUrl == null)
            return BadRequest("Tải ảnh không thành công.");

        // Cập nhật ảnh hồ sơ
        var userDto = await _userService.UpdateProfileImageAsync(id, imageUrl);
        if (userDto == null)
            return NotFound("Không tìm thấy người dùng.");

        return Ok(new { ImageUrl = userDto.ProfileImage });
    }
}