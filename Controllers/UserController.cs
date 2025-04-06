using API_WebH3.DTOs.User;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly StudentService _studentService;

    public UserController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("profile/")]
   public IActionResult GetUserInfo()
   {
      // Lấy thông tin người dùng từ claims trong token
      var userName = User.FindFirst(ClaimTypes.Name)?.Value;
      var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
      var userImage = User.FindFirst("profileImage")?.Value;
        var userBirthDate = User.FindFirst("birthDate")?.Value;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
      {
         return Unauthorized("Không tìm thấy thông tin người dùng.");
      }

      var userInfo = new
      {
        fullname = userName,
        Email = userEmail,
        Role = userRole,
        ProfileImage = userImage,
        BirthDate = userBirthDate,
      };

      return Ok(userInfo);
   }
    [HttpPut("profile/{id}")]
    public async Task<IActionResult> UpdateStudent(UpdateStudentDto updateStudentDto, string id)
    {
        var student = await _studentService.UpdateStudentAsync(updateStudentDto, id);
        return Ok(student);
    }


    [HttpPost("profile/upload-avatar/{id}")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, string id)
    {
        var result = await _studentService.UploadAvatarAsync(file, id);
        return Ok(result);
    }

    

}