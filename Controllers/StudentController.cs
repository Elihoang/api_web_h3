using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_WebH3.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
   private readonly StudentService _studentService;

   public StudentController(StudentService studentService)
   {
      _studentService = studentService;
   }

   [HttpGet]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> GetAllStudents()
   {
      var students = await _studentService.GetAllStudentsAsync();
      return Ok(students);
   }

   [HttpGet("{id}")]
   public async Task<IActionResult> GetStudentById(string id)
   {
      var student = await _studentService.GetStudentByIdAsync(id);
      return Ok(student);
   }

   [HttpPost]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> CreateStudent(User user)
   {
      var student = await _studentService.CreateStudentAsync(user);
      return Ok(student);
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> UpdateStudent(string id, User user)
   {
      var student = await _studentService.UpdateStudentAsync(user);
      return Ok(student);
   }

   [HttpDelete("{id}")]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> DeleteStudent(string id)
   {
      var student = await _studentService.DeleteStudentAsync(id);
      return Ok(student);
   }

   [HttpPost("upload-avatar/{id}")]
   [Authorize]
   public async Task<IActionResult> UploadAvatar(IFormFile file, string id)
   {
      var result = await _studentService.UploadAvatarAsync(file, id);
      return Ok(result);
   }
}
