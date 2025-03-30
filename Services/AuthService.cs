using API_WebH3.DTOs.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace API_WebH3.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        // ✅ Lưu OTP trong RAM, dùng ConcurrentDictionary để tránh lỗi đồng bộ
        // ✅ Dùng static để lưu OTP trong RAM
        private static readonly ConcurrentDictionary<string, (string ResetCode, DateTime ExpiryTime)> _resetCodes = new();


        public AuthService(IUserRepository userRepository, IConfiguration configuration, EmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return null;

            string token = GenerateJwtToken(user);
            return new AuthResponseDto { Token = token, Role = user.Role };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                return false; // Email đã tồn tại

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                BirthDate = registerDto.BirthDate ?? null,
                ProfileImage = registerDto.ProfileImage,
                Role = registerDto.Role
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AccountUserDto> GetUserDetailsAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new AccountUserDto
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                BirthDate = user.BirthDate
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            var resetCode = new Random().Next(100000, 999999).ToString();
            _resetCodes[email] = (resetCode, DateTime.UtcNow.AddMinutes(10));

            Console.WriteLine($" OTP: {_resetCodes[email].ResetCode}  email: {email}");

            string subject = "Khôi phục mật khẩu";
            string body = $"Mã OTP của bạn là: <b>{resetCode}</b>. Mã có hiệu lực trong 10 phút.";
            await _emailService.SendPasswordResetEmailAsync(email, subject, body);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            if (!_resetCodes.TryGetValue(email, out var storedCode))
            {
                Console.WriteLine($" OTP : {email}");
                return false;
            }

            Console.WriteLine($"{storedCode.ResetCode} -  {resetCode}");

            if (storedCode.ResetCode != resetCode || storedCode.ExpiryTime < DateTime.UtcNow)
            {
                Console.WriteLine($" Mã OTP không hợp lệ hoặc đã hết hạn cho email: {email}");
                return false;
            }

            // ✅ Xóa mã OTP sau khi sử dụng
            _resetCodes.TryRemove(email, out _);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}

    

