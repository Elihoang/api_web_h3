using API_WebH3.DTOs.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace API_WebH3.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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
    }
}
