using API_WebH3.DTO.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;
using API_WebH3.Helpers;
using Microsoft.Extensions.Logging;

namespace API_WebH3.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;

        private static readonly ConcurrentDictionary<string, (string ResetCode, DateTime ExpiryTime)> _resetCodes =
            new();

        public AuthService(IUserRepository userRepository, IConfiguration configuration, EmailService emailService,
            IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
        }

        public async Task<(AuthResponseDto? Result, string? ErrorMessage)> LoginAsync(Login loginDto)
        {
            try
            {
                AppLogger.LogInfo($"Logging in user: {loginDto.Email}");
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    AppLogger.LogError($"User not found: {loginDto.Email}");
                    return (null, "Email không tồn tại");
                }

                AppLogger.LogInfo($"Verifying password for: {loginDto.Email}");
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    AppLogger.LogError($"Invalid password for: {loginDto.Email}");
                    return (null, "Mật khẩu không đúng");
                }

                AppLogger.LogInfo($"Generating JWT for: {loginDto.Email}");
                string token = GenerateJwtToken(user);
                return (new AuthResponseDto { Token = token, Role = user.Role }, null);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Login error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> RegisterAsync(Register registerDto)
        {
            try
            {
                AppLogger.LogInfo($"Registering user: {registerDto.Email}");
                if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                {
                    AppLogger.LogError($"Email already exists: {registerDto.Email}");
                    return false;
                }

                var user = new User
                {
                    FullName = registerDto.FullName,
                    Email = registerDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    BirthDate = registerDto.BirthDate ?? null,
                    ProfileImage = registerDto.ProfileImage,
                    Role = registerDto.Role,
                    Phone = registerDto.Phone
                };

                AppLogger.LogInfo($"Adding user: {user.Email}");
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
                AppLogger.LogInfo($"User registered: {user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Register error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("profileImage", user.ProfileImage ?? string.Empty),
                    new Claim("birthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            AppLogger.LogInfo($"Generated JWT for {user.Email}: {tokenString}");
            return tokenString;
        }

        public async Task<UserDto> GetUserDetailsAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                BirthDate = user.BirthDate,
                ProfileImage = user.ProfileImage,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IpAddress = user.IpAddress,
                DeviceName = user.DeviceName,
                GoogleId = user.GoogleId,
                IsGoogleAccount = user.IsGoogleAccount
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return false;
                }

                var resetCode = new Random().Next(100000, 999999).ToString();
                _resetCodes[email] = (resetCode, DateTime.UtcNow.AddMinutes(10));
                
                var filePath = Path.Combine(_env.WebRootPath, "templates", "OtpTemplate.html");
                if (!File.Exists(filePath))
                {
                    AppLogger.LogError($"Template not found: {filePath}");
                }

                string subject = "Khôi phục mật khẩu";
                var emailBody = await File.ReadAllTextAsync(filePath);
                emailBody = emailBody.Replace("{{OTP}}", resetCode)
                    .Replace("{fullName}", user.FullName);

                await _emailService.SendPasswordResetEmailAsync(email, subject, emailBody);
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"ForgotPassword error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            try
            {
                AppLogger.LogInfo($"Resetting password for: {email}");
                if (!_resetCodes.TryGetValue(email, out var storedCode))
                {
                    AppLogger.LogError($"No OTP found for: {email}");
                    return false;
                }

                AppLogger.LogInfo($"Stored OTP: {storedCode.ResetCode}, Provided OTP: {resetCode}");
                if (storedCode.ResetCode != resetCode || storedCode.ExpiryTime < DateTime.UtcNow)
                {
                    AppLogger.LogError($"Invalid or expired OTP for: {email}");
                    return false;
                }

                _resetCodes.TryRemove(email, out _);

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    AppLogger.LogError($"User not found: {email}");
                    return false;
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _userRepository.SaveChangesAsync();
                AppLogger.LogInfo($"Password reset for: {email}");
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"ResetPassword error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}