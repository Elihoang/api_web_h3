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
            BirthDate = s.BirthDate.HasValue
            ? s.BirthDate.Value.ToString("dd/MM/yyyy")
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
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            BirthDate = user.BirthDate.HasValue
            ? user.BirthDate.Value.ToString("yyyy-MM-dd")
            : null,
            ProfileImage = user.ProfileImage,
            Role = user.Role
         };
      }

      public async Task<StudentDto> CreateStudentAsync(CreateStudentDto user)
      {
            var newStudent = new User
            {
            FullName = user.FullName,
            Email = user.Email,
            Password = user.Password,
            Role = user.Role,
            BirthDate = user.BirthDate != null ? DateTime.Parse(user.BirthDate) : null,
            Phone = user.Phone
         };

         if (user.BirthDate != null && user.BirthDate != "")
         {
            newStudent.BirthDate = DateTime.Parse(user.BirthDate);
         }
         
         await _studentRepository.CreateAsync(newStudent);
         return new StudentDto
         {
            Id = newStudent.Id.ToString(),
            FullName = newStudent.FullName,
            Email = newStudent.Email,
            BirthDate = newStudent.BirthDate.HasValue
             ? newStudent.BirthDate.Value.ToString("yyyy-MM-dd")
             : null,
            ProfileImage = newStudent.ProfileImage,
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

            existingStudent.FullName = updateStudentDto.FullName;
            existingStudent.Email = updateStudentDto.Email;

            if (updateStudentDto.BirthDate != null)
            {
                // Phân tích chuỗi thời gian và chuyển sang UTC
                var parsedDate = DateTime.Parse(updateStudentDto.BirthDate);
                existingStudent.BirthDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            // Chỉ cập nhật mật khẩu nếu có giá trị mới và mã hóa nó
            if (!string.IsNullOrEmpty(updateStudentDto.Password))
            {
                existingStudent.Password = BCrypt.Net.BCrypt.HashPassword(updateStudentDto.Password);
            }

            await _studentRepository.UpdateAsync(existingStudent);

            return new UpdateStudentDto
            {
                FullName = existingStudent.FullName,
                Email = existingStudent.Email,
                BirthDate = existingStudent.BirthDate.HasValue
                    ? existingStudent.BirthDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    : null
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