using API_WebH3.DTOs.User;
using API_WebH3.Models;
using System.Security.Claims;
using System.Text;
using API_WebH3.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace API_WebH3.Services
{
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
            BirthDate = s.BirthDate.HasValue
            ? s.BirthDate.Value.ToString("dd-MM-yyyy")
            : null,
            ProfileImage = s.ProfileImage,
            Role = s.Role
         });
      }

      public async Task<StudentDto> GetStudentByIdAsync(string id)
      {
         var user = await _studentRepository.GetByIdAsync(id);
         return new StudentDto
         {
            FullName = user.FullName,
            Email = user.Email,
            BirthDate = user.BirthDate.HasValue
            ? user.BirthDate.Value.ToString("dd-MM-yyyy")
            : null,
            ProfileImage = user.ProfileImage
         };
      }

      public async Task<StudentDto> CreateStudentAsync(User user)
      {
         var newStudent = new User
         {
            FullName = user.FullName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            ProfileImage = user.ProfileImage,
            Password = user.Password,
            Role = user.Role
         };
         await _studentRepository.CreateAsync(newStudent);
         return new StudentDto
         {
            FullName = newStudent.FullName,
            Email = newStudent.Email,
            BirthDate = newStudent.BirthDate.HasValue
             ? newStudent.BirthDate.Value.ToString("dd-MM-yyyy")
             : null,
            ProfileImage = newStudent.ProfileImage
         };
      }

      public async Task<StudentDto> UpdateStudentAsync(User user)
      {
         var existingStudent = await _studentRepository.GetByIdAsync(user.Id.ToString());
         if (existingStudent == null)
         {
            throw new Exception("Student not found");
         }
         existingStudent.FullName = user.FullName;
         existingStudent.Email = user.Email;
         existingStudent.BirthDate = user.BirthDate;
         existingStudent.ProfileImage = user.ProfileImage;
         existingStudent.Password = user.Password;
         existingStudent.Role = user.Role;
         await _studentRepository.UpdateAsync(existingStudent);
         return new StudentDto
         {
            FullName = existingStudent.FullName,
            Email = existingStudent.Email,
            BirthDate = existingStudent.BirthDate.HasValue
            ? existingStudent.BirthDate.Value.ToString("dd-MM-yyyy")
            : null,
            ProfileImage = existingStudent.ProfileImage
         };

      }

      public async Task<bool> DeleteStudentAsync(string id)
      {
         return await _studentRepository.DeleteAsync(id);
      }

      public async Task<string> UploadAvatarAsync(IFormFile file, string id)
      {
         if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded");

         // Tạo thư mục uploads nếu chưa có
         string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
         if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

         // Tạo tên file duy nhất
         string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
         string filePath = Path.Combine(uploadsFolder, uniqueFileName);

         // Lưu file vào thư mục
         using (var stream = new FileStream(filePath, FileMode.Create))
         {
            await file.CopyToAsync(stream);
         }

         var user = await _studentRepository.GetByIdAsync(id);
         if (user != null)
         {
            user.ProfileImage = "/uploads/" + uniqueFileName;
            await _studentRepository.UpdateAsync(user);
         }

         return "/uploads/" + uniqueFileName;
      }
   }
}