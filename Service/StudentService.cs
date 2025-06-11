using API_WebH3.DTO.User;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class StudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IWebHostEnvironment _env;

    public StudentService(IStudentRepository studentRepository, IWebHostEnvironment env)
    {
        _studentRepository = studentRepository;
        _env = env;
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(s => new StudentDto
        {
            Id = s.Id.ToString(),
            FullName = s.FullName,
            Email = s.Email,
            BirthDate = s.BirthDate,
            ProfileImage = s.ProfileImage,
            Phone =  s.Phone,
            Role = s.Role
        });
    }

    public async Task<StudentDto> GetStudentByIdAsync(string id)
    {
        var user = await _studentRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new Exception("Student not found");
        }
        return new StudentDto
        {
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            ProfileImage = user.ProfileImage,
            Phone = user.Phone,
            Role = user.Role
        };
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto user)
    {
        var newStudent = new User
        {
            FullName = user.FullName,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Role = user.Role,
            BirthDate = user.BirthDate.HasValue
                ? DateTime.SpecifyKind(user.BirthDate.Value, DateTimeKind.Utc)
                : null,
            ProfileImage = user.ProfileImage,
            Phone = user.Phone,
        };

        await _studentRepository.CreateAsync(newStudent);
        return new StudentDto
        {
            Id = newStudent.Id.ToString(),
            FullName = newStudent.FullName,
            Email = newStudent.Email,
            BirthDate = newStudent.BirthDate,
            ProfileImage = newStudent.ProfileImage,
            Phone = newStudent.Phone,
            Role = newStudent.Role
        };
    }

    public async Task<UpdateStudentDto> UpdateStudentAsync(UpdateStudentDto updateStudentDto, string id)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(id);
        if (existingStudent == null)
        {
            throw new Exception("Student not found");
        }

        // Chỉ cập nhật các trường được gửi
        existingStudent.FullName = updateStudentDto.FullName ?? existingStudent.FullName;
        existingStudent.Email = updateStudentDto.Email ?? existingStudent.Email;

        // Cập nhật BirthDate chỉ nếu có giá trị
        if (updateStudentDto.BirthDate.HasValue)
        {
            existingStudent.BirthDate = DateTime.SpecifyKind(updateStudentDto.BirthDate.Value, DateTimeKind.Utc);
        }
        
        if (!string.IsNullOrEmpty(updateStudentDto.ProfileImage))
        {
            existingStudent.ProfileImage = updateStudentDto.ProfileImage;
        }
        
        if (!string.IsNullOrEmpty(updateStudentDto.Phone))
        {
            existingStudent.Phone = updateStudentDto.Phone;
        }
        
        if (!string.IsNullOrEmpty(updateStudentDto.Password))
        {
            existingStudent.Password = BCrypt.Net.BCrypt.HashPassword(updateStudentDto.Password);
        }

        await _studentRepository.UpdateAsync(existingStudent);

        return new UpdateStudentDto
        {
            FullName = existingStudent.FullName,
            Email = existingStudent.Email,
            Phone = existingStudent.Phone,
            ProfileImage = existingStudent.ProfileImage,
            BirthDate = existingStudent.BirthDate,
            Password = null 
        };
    }

    public async Task<bool> DeleteStudentAsync(string id)
    {
        return await _studentRepository.DeleteAsync(id);
    }
}