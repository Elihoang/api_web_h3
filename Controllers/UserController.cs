using API_WebH3.DTOs.User;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
   public UserController()
   {}

   [HttpGet("profile")]
   public IActionResult GetUserInfo()
   {
      // Lấy thông tin người dùng từ claims trong token
      var userName = User.FindFirst(ClaimTypes.Name)?.Value;
      var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
      {
         return Unauthorized("Không tìm thấy thông tin người dùng.");
      }

      var userInfo = new
      {
         fullname = userName,
         Email = userEmail,
         Role = userRole
      };

      return Ok(userInfo);
   }

}