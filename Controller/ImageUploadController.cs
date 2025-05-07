using System;
using System.IO;
using System.Threading.Tasks;
using API_WebH3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_WebH3.Models;
using API_WebH3.Service;

namespace API_WebH3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageUploadController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("user/{userId}/profile-image")]
        public async Task<IActionResult> UploadUserProfileImage(Guid userId, IFormFile image)
        {
            try
            {
                var imageUrl = await _imageService.UploadUserProfileImageAsync(userId, image);
                return Ok(new { ProfileImageUrl = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Đã xảy ra lỗi khi tải ảnh lên.");
            }
        }

        [HttpPost("post/{postId}/image")]
        public async Task<IActionResult> UploadPostImage(Guid postId, IFormFile image)
        {
            try
            {
                var imageUrl = await _imageService.UploadPostImageAsync(postId, image);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Đã xảy ra lỗi khi tải ảnh lên.");
            }
        }
    }
}