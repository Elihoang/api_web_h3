using API_WebH3.DTO.User;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public AuthController(AuthService authService, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _authService = authService;
        _environment = environment;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register registerDto)
    {
        try
        {
            bool success = await _authService.RegisterAsync(registerDto);
            if (!success)
                return BadRequest(new { message = "Email already exists" });

            var loginDto = new Login { Email = registerDto.Email, Password = registerDto.Password };
            var (result, errorMessage) = await _authService.LoginAsync(loginDto);
            if (result == null)
                return StatusCode(500, new { message = errorMessage ?? "Failed to login after registration" });

            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]);
            return Ok(new
            {
                message = "User registered successfully",
                role = result.Role,
                email = registerDto.Email,
                token = result.Token,
                expiresInMinutes = expirationMinutes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login loginDto)
    {
        try
        {
            var (result, errorMessage) = await _authService.LoginAsync(loginDto);
            if (result == null)
                return BadRequest(new { message = errorMessage ?? "Thông tin đăng nhập không hợp lệ" });

            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]);
            return Ok(new
            {
                message = "Login successful",
                role = result.Role,
                token = result.Token,
                email = loginDto.Email,
                expiresInMinutes = expirationMinutes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("test-hash")]
    public IActionResult TestHash()
    {
        string hash = BCrypt.Net.BCrypt.HashPassword("12345");
        return Ok(new { Hash = hash });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            var userRoleClaim = User.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { message = "Token không hợp lệ." });

            var userDto = await _authService.GetUserDetailsAsync(userId);
            if (userDto == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy thông tin người dùng: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new { message = "Đã xảy ra lỗi server." });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            bool success = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            if (!success)
                return NotFound(new { message = "Không tìm thấy email trong hệ thống!" });

            return Ok(new { message = "Mã OTP đã được gửi qua email!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            bool success = await _authService.ResetPasswordAsync(
                resetPasswordDto.Email,
                resetPasswordDto.ResetCode,
                resetPasswordDto.NewPassword
            );
            if (!success)
                return BadRequest(new { message = "Mã xác nhận không hợp lệ hoặc đã hết hạn!" });

            return Ok(new { message = "Mật khẩu đã được cập nhật thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}