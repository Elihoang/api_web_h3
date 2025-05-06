using API_WebH3.DTO.User;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration; // Thêm IConfiguration

    public AuthController(AuthService authService, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _authService = authService;
        _environment = environment;
        _configuration = configuration; // Tiêm IConfiguration
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register registerDto)
    {
        try
        {
            bool success = await _authService.RegisterAsync(registerDto);
            if (!success)
                return BadRequest(new { message = "Email already exists" });

            // Đăng nhập tự động sau khi đăng ký
            var loginDto = new Login { Email = registerDto.Email, Password = registerDto.Password };
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return StatusCode(500, new { message = "Failed to login after registration" });

            // Lưu token vào cookie
            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _environment.IsDevelopment() ? false : true, // Tắt Secure trong dev
                SameSite = SameSiteMode.None, // Cho phép cross-site
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
            Response.Cookies.Append("auth_token", result.Token, cookieOptions);

            return Ok(new { message = "User registered successfully", role = result.Role });
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
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Lưu token vào cookie
            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _environment.IsDevelopment() ? false : true, // Tắt Secure trong dev
                SameSite = SameSiteMode.None, // Cho phép cross-site
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
            Response.Cookies.Append("auth_token", result.Token, cookieOptions);

            return Ok(new { message = "Login successful", role = result.Role, token = result.Token });
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

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
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