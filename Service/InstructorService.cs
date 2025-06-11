using API_WebH3.DTO.User;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class InstructorService
{
    
    private readonly IInstructorRepository _instructorRepository;
    private readonly IWebHostEnvironment _env;
    
    public InstructorService(IInstructorRepository instructorRepository, IWebHostEnvironment env)
    {
        _instructorRepository = instructorRepository;
        _env = env;
    }
    
    public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();
        return instructors.Select(i => new InstructorDto
        {
            Id = i.Id,
            FullName = i.FullName,
            Email = i.Email,
            Phone = i.Phone,
            BirthDate = i.BirthDate,
            ProfileImage = i.ProfileImage,
            Role = i.Role,
            CreatedAt = i.CreatedAt,
        });
    }
    public async Task<InstructorDto> GetInstructorByIdAsync(Guid id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);
        if (instructor == null)
        {
            return null;
        }
        return new InstructorDto
        {
            Id = instructor.Id,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Phone = instructor.Phone,
            BirthDate = instructor.BirthDate,
            ProfileImage = instructor.ProfileImage,
            Role = instructor.Role,
            CreatedAt = instructor.CreatedAt,
        };
    }
    public async Task<InstructorDto> CreateInstructorAsync(CreateInstructorDto createInstructorDto)
    {
        if (createInstructorDto.Role != "Instructor")
        {
            throw new ArgumentException("Role phải là Instructor.");
        }

        // Kiểm tra email trùng lặp
        var existingUser = await _instructorRepository.GetByEmailAsync(createInstructorDto.Email);
        if (existingUser != null)
        {
            throw new ArgumentException("Email đã được sử dụng.");
        }

        var newInstructor = new User
        {
            Id = Guid.NewGuid(),
            FullName = createInstructorDto.FullName,
            Email = createInstructorDto.Email,
            Password = !string.IsNullOrEmpty(createInstructorDto.Password)
                ? BCrypt.Net.BCrypt.HashPassword(createInstructorDto.Password)
                : null,
            Phone = createInstructorDto.Phone,
            BirthDate = createInstructorDto.BirthDate.HasValue
                ? DateTime.SpecifyKind(createInstructorDto.BirthDate.Value, DateTimeKind.Utc)
                : null,
            ProfileImage = createInstructorDto.ProfileImage,
            Role = createInstructorDto.Role,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
        };

        await _instructorRepository.CreateAsync(newInstructor);

        return new InstructorDto
        {
            Id = newInstructor.Id,
            FullName = newInstructor.FullName,
            Email = newInstructor.Email,
            Phone = newInstructor.Phone,
            BirthDate = newInstructor.BirthDate,
            ProfileImage = newInstructor.ProfileImage,
            Role = newInstructor.Role,
            CreatedAt = newInstructor.CreatedAt,

        };
    }
    public async Task<InstructorDto> UpdateInstructorAsync(Guid id, UpdateInstructorDto updateInstructorDto)
    {
        var existingInstructor = await _instructorRepository.GetByIdAsync(id);
        if (existingInstructor == null)
        {
            return null;
        }
        // Kiểm tra email trùng lặp (nếu email thay đổi)
        if (updateInstructorDto.Email != existingInstructor.Email)
        {
            var existingUser = await _instructorRepository.GetByEmailAsync(updateInstructorDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("Email đã được sử dụng.");
            }
        }

        existingInstructor.FullName = updateInstructorDto.FullName;
        existingInstructor.Email = updateInstructorDto.Email;
        if (!string.IsNullOrEmpty(updateInstructorDto.Password))
        {
            existingInstructor.Password = BCrypt.Net.BCrypt.HashPassword(updateInstructorDto.Password);
        }
        existingInstructor.Phone = updateInstructorDto.Phone;
        existingInstructor.BirthDate = updateInstructorDto.BirthDate.HasValue
            ? DateTime.SpecifyKind(updateInstructorDto.BirthDate.Value, DateTimeKind.Utc)
            : null;
        if (!string.IsNullOrEmpty(updateInstructorDto.ProfileImage))
        {
            existingInstructor.ProfileImage = updateInstructorDto.ProfileImage;
        }

        await _instructorRepository.UpdateAsync(existingInstructor);

        return new InstructorDto
        {
            Id = existingInstructor.Id,
            FullName = existingInstructor.FullName,
            Email = existingInstructor.Email,
            Phone = existingInstructor.Phone,
            BirthDate = existingInstructor.BirthDate,
            ProfileImage = existingInstructor.ProfileImage,
            Role = existingInstructor.Role,
            CreatedAt = existingInstructor.CreatedAt,
        };
    }
    public async Task<bool> DeleteInstructorAsync(Guid id)
    {
        return await _instructorRepository.DeleteAsync(id);
    }
    public async Task<string> UploadAvatarAsync(IFormFile file, Guid id)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Không có tệp được tải lên.");

        string uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var instructor = await _instructorRepository.GetByIdAsync(id);
        if (instructor == null)
        {
            throw new Exception("Không tìm thấy Instructor.");
        }

        instructor.ProfileImage = "/Uploads/" + uniqueFileName;
        await _instructorRepository.UpdateAsync(instructor);

        return "/Uploads/" + uniqueFileName;
    }
}