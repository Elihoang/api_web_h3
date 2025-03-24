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
}