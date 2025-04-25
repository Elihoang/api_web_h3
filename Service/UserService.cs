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

        user.FullName = updateUserDto.FullName;
        user.Email = updateUserDto.Email;
        user.Password = updateUserDto.Password;
        user.Phone = updateUserDto.Phone;
        user.BirthDate = updateUserDto.BirthDate.HasValue
            ? DateTime.SpecifyKind(updateUserDto.BirthDate.Value, DateTimeKind.Utc)
            : null;
        user.ProfileImage = updateUserDto.ProfileImage;
        user.Role = updateUserDto.Role;
        user.IpAddress = updateUserDto.IpAddress;
        user.DeviceName = updateUserDto.DeviceName;
        user.GoogleId = updateUserDto.GoogleId;
        user.IsGoogleAccount = updateUserDto.IsGoogleAccount;

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
}