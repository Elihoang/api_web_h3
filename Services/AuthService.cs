﻿using API_WebH3.DTOs.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;


namespace API_WebH3.Services
{
   public class AuthService
   {
      private readonly IUserRepository _userRepository;
      private readonly IConfiguration _configuration;
      private readonly EmailService _emailService;
      private readonly IWebHostEnvironment _env;
      private static readonly ConcurrentDictionary<string, (string ResetCode, DateTime ExpiryTime)> _resetCodes = new();


      public AuthService(IUserRepository userRepository, IConfiguration configuration, EmailService emailService, IWebHostEnvironment env)
      {
         _userRepository = userRepository;
         _configuration = configuration;
         _emailService = emailService;
         _env = env;
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
            return false;

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
               new Claim("id", user.Id.ToString()),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role, user.Role),
               new Claim(ClaimTypes.Name, user.FullName),
               new Claim("profileImage", user.ProfileImage ?? string.Empty),
               new Claim("birthDate", user.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
         };

         var token = tokenHandler.CreateToken(tokenDescriptor);
         return tokenHandler.WriteToken(token);
      }
      public async Task<AccountUserDto> GetUserDetailsAsync(string email)
      {
         // Correctly awaiting the result of GetByEmailAsync to get the User object
         var user = await _userRepository.GetByEmailAsync(email);

         if (user == null)
         {
            throw new Exception("User not found");
         }

         // Map the user entity to the DTO
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

         var filePath = Path.Combine(_env.WebRootPath, "templates", "OtpTemplate.html");
         if (!File.Exists(filePath)) throw new FileNotFoundException("Email template not found");

         string subject = "Khôi phục mật khẩu";

         var emailBody = await File.ReadAllTextAsync(filePath);
         emailBody = emailBody.Replace("{{OTP}}", resetCode);
         
         await _emailService.SendPasswordResetEmailAsync(email, subject, emailBody);

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


