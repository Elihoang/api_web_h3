using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API_WebH3.Models;
using API_WebH3.Repository;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;

        public EmailController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmails()
        {
            try
            {
                var emails = await _emailRepository.GetAllEmailsAsync();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách email: {ex.Message}");
                return StatusCode(500, "Lỗi server khi lấy danh sách email");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailById(int id)
        {
            try
            {
                var email = await _emailRepository.GetEmailByIdAsync(id);
                if (email == null)
                {
                    return NotFound("Email không tồn tại");
                }
                return Ok(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy email theo Id: {ex.Message}");
                return StatusCode(500, "Lỗi server khi lấy email");
            }
        }

        [HttpGet("by-type/{sourceType}")]
        public async Task<IActionResult> GetEmailsBySourceType(string sourceType)
        {
            try
            {
                var emails = await _emailRepository.GetEmailsBySourceTypeAsync(sourceType);
                if (emails == null || emails.Count == 0)
                {
                    return NotFound("Không tìm thấy email với SourceType này");
                }
                return Ok(emails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy email theo SourceType: {ex.Message}");
                return StatusCode(500, "Lỗi server khi lấy email");
            }
        }
         
        [HttpGet("paginated")]
        public async Task<ActionResult<IEnumerable<Email>>> GetPaginatedEmail([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var email = await _emailRepository.GetAllEmailsAsync();
            var totalItems = email.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedEmailList = email.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                Data = pagedEmailList,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }
    }
}