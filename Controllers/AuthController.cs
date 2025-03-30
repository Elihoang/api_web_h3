using API_WebH3.DTOs.User;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        bool success = await _authService.RegisterAsync(registerDto);
        if (!success)
            return BadRequest(new { message = "Email already exists" });

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(result);
    }
    [HttpGet("test")]
    public IActionResult TestApi()
    {
        return Ok(new { message = "API is running" });
    }
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile([FromQuery] string email)
    {
        try
        {
            var userDetails = await _authService.GetUserDetailsAsync(email);
            return Ok(userDetails);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        bool success = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
        if (!success)
            return NotFound(new { message = "Không tìm thấy email trong hệ thống!" });

        return Ok(new { message = "Mã OTP đã được gửi qua email!" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        bool success = await _authService.ResetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.ResetCode, resetPasswordDto.NewPassword);
        if (!success)
            return BadRequest(new { message = "Mã xác nhận không hợp lệ hoặc đã hết hạn!" });

        return Ok(new { message = "Mật khẩu đã được cập nhật thành công!" });
    }
}