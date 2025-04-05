using API_WebH3.DTOs.Payment;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Lấy tất cả thanh toán
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentDto>>> GetAllAsync()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Lấy thanh toán theo ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> GetByIdAsync(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    /// <summary>
    /// Tạo thanh toán mới
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> CreateAsync(CreatePaymentDto createPaymentDto)
    {
        var payment = await _paymentService.CreateAsync(createPaymentDto);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = payment.Id }, payment);
    }

    /// <summary>
    /// Cập nhật thanh toán
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentDto>> UpdateAsync(Guid id, UpdatePaymentDto updatePaymentDto)
    {
        var payment = await _paymentService.UpdateAsync(id, updatePaymentDto);
        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    /// <summary>
    /// Xóa thanh toán
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var result = await _paymentService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Lấy thanh toán của một người dùng
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<PaymentDto>>> GetByUserIdAsync(Guid userId)
    {
        var payments = await _paymentService.GetByUserIdAsync(userId);
        return Ok(payments);
    }

    /// <summary>
    /// Lấy thanh toán của một khóa học
    /// </summary>
    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<List<PaymentDto>>> GetByCourseIdAsync(Guid courseId)
    {
        var payments = await _paymentService.GetByCourseIdAsync(courseId);
        return Ok(payments);
    }

    /// <summary>
    /// Lấy thanh toán theo trạng thái
    /// </summary>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentDto>>> GetByStatusAsync(string status)
    {
        var payments = await _paymentService.GetByStatusAsync(status);
        return Ok(payments);
    }
}