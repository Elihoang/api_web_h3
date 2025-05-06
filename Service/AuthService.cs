using API_WebH3.DTO.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repository;
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

        private static readonly ConcurrentDictionary<string, (string ResetCode, DateTime ExpiryTime)> _resetCodes =
            new();

        public AuthService(IUserRepository userRepository, IConfiguration configuration, EmailService emailService,
            IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
            Console.WriteLine("AuthService initialized");
        }

        public async Task<AuthResponseDto?> LoginAsync(Login loginDto)
        {
            try
            {
                Console.WriteLine($"Logging in user: {loginDto.Email}");
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {loginDto.Email}");
                    return null;
                }

                Console.WriteLine($"Verifying password for: {loginDto.Email}");
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    Console.WriteLine($"Invalid password for: {loginDto.Email}");
                    return null;
                }

                Console.WriteLine($"Generating JWT for: {loginDto.Email}");
                string token = GenerateJwtToken(user);
                return new AuthResponseDto { Token = token, Role = user.Role };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> RegisterAsync(Register registerDto)
        {
            try
            {
                Console.WriteLine($"Registering user: {registerDto.Email}");
                if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                {
                    Console.WriteLine($"Email already exists: {registerDto.Email}");
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

                Console.WriteLine($"Adding user: {user.Email}");
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
                Console.WriteLine($"User registered: {user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}\n{ex.StackTrace}");
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
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), // Sử dụng cấu hình
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            Console.WriteLine($"Generated JWT for {user.Email}: {tokenString}");
            return tokenString;
        }

        public async Task<AccountUserDto> GetUserDetailsAsync(string email)
        {
            try
            {
                Console.WriteLine($"Fetching user details: {email}");
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {email}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserDetails error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                Console.WriteLine($"Forgot password for: {email}");
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {email}");
                    return false;
                }

                var resetCode = new Random().Next(100000, 999999).ToString();
                _resetCodes[email] = (resetCode, DateTime.UtcNow.AddMinutes(10));

                Console.WriteLine($"OTP: {_resetCodes[email].ResetCode} for email: {email}");

                var filePath = Path.Combine(_env.WebRootPath, "templates", "OtpTemplate.html");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Template not found: {filePath}");
                    throw new FileNotFoundException("Email template not found");
                }

                string subject = "Khôi phục mật khẩu";
                var emailBody = await File.ReadAllTextAsync(filePath);
                emailBody = emailBody.Replace("{{OTP}}", resetCode);

                await _emailService.SendPasswordResetEmailAsync(email, subject, emailBody);
                Console.WriteLine($"OTP email sent to: {email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ForgotPassword error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            try
            {
                Console.WriteLine($"Resetting password for: {email}");
                if (!_resetCodes.TryGetValue(email, out var storedCode))
                {
                    Console.WriteLine($"No OTP found for: {email}");
                    return false;
                }

                Console.WriteLine($"Stored OTP: {storedCode.ResetCode}, Provided OTP: {resetCode}");
                if (storedCode.ResetCode != resetCode || storedCode.ExpiryTime < DateTime.UtcNow)
                {
                    Console.WriteLine($"Invalid or expired OTP for: {email}");
                    return false;
                }

                _resetCodes.TryRemove(email, out _);

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {email}");
                    return false;
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _userRepository.SaveChangesAsync();
                Console.WriteLine($"Password reset for: {email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ResetPassword error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}