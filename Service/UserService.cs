using API_WebH3.DTO.User;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            BirthDate = u.BirthDate,
            ProfileImage = u.ProfileImage,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            IpAddress = u.IpAddress,
            DeviceName = u.DeviceName,
            GoogleId = u.GoogleId,
            IsGoogleAccount = u.IsGoogleAccount
        });
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
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

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = createUserDto.FullName,
            Email = createUserDto.Email,
            Password = createUserDto.Password,
            Phone = createUserDto.Phone,
            BirthDate = createUserDto.BirthDate.HasValue
                ? DateTime.SpecifyKind(createUserDto.BirthDate.Value, DateTimeKind.Utc)
                : null,
            ProfileImage = createUserDto.ProfileImage,
            Role = createUserDto.Role,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            IpAddress = createUserDto.IpAddress,
            DeviceName = createUserDto.DeviceName,
            GoogleId = createUserDto.GoogleId,
            IsGoogleAccount = createUserDto.IsGoogleAccount
        };

        await _userRepository.AddAsync(user);

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

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        if (updateUserDto.FullName != null)
        {
            user.FullName = updateUserDto.FullName;
        }
        if (updateUserDto.Email != null)
        {
            user.Email = updateUserDto.Email;
        }
        if (updateUserDto.Password != null)
        {
            user.Password = updateUserDto.Password;
        }
        if (updateUserDto.Phone != null)
        {
            user.Phone = updateUserDto.Phone;
        }
        if (updateUserDto.BirthDate.HasValue) // Chỉ cập nhật nếu BirthDate được gửi
        {
            user.BirthDate = DateTime.SpecifyKind(updateUserDto.BirthDate.Value, DateTimeKind.Utc);
        }
        if (updateUserDto.ProfileImage != null)
        {
            user.ProfileImage = updateUserDto.ProfileImage;
        }
        if (updateUserDto.Role != null)
        {
            user.Role = updateUserDto.Role;
        }
        if (updateUserDto.IpAddress != null)
        {
            user.IpAddress = updateUserDto.IpAddress;
        }
        if (updateUserDto.DeviceName != null)
        {
            user.DeviceName = updateUserDto.DeviceName;
        }
        if (updateUserDto.GoogleId != null)
        {
            user.GoogleId = updateUserDto.GoogleId;
        }
        if (updateUserDto.IsGoogleAccount)
        {
            user.IsGoogleAccount = updateUserDto.IsGoogleAccount;
        }
        
        await _userRepository.UpdateAsync(user);

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

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, UpdatePasswordDto updatePasswordDto)
    {
        if (updatePasswordDto == null)
        {
            AppLogger.LogError("Dữ liệu cập nhật mật khẩu không được null.");
            throw new ArgumentNullException(nameof(updatePasswordDto), "Dữ liệu cập nhật không được null.");
        }

        if (updatePasswordDto.Password != updatePasswordDto.ConfirmPassword)
        {
            AppLogger.LogError("Mật khẩu xác nhận không khớp.");
            throw new ArgumentException("Mật khẩu xác nhận không khớp.");
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        try
        {
            // Băm mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.Password);

            AppLogger.LogSuccess($"Đã cập nhật mật khẩu cho người dùng: {user.Id}");

            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Lỗi khi cập nhật mật khẩu: {ex.Message}");
            return false;
        }
    }
    public async Task<UserDto> UpdateProfileImageAsync(Guid id, string profileImage)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.ProfileImage = profileImage;
        await _userRepository.UpdateAsync(user);

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
}