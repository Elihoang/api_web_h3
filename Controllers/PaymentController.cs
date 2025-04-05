using API_WebH3.DTOs.Enrollment;
using API_WebH3.DTOs.Order;
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
    private readonly VnpayService _vnpayService;
    public PaymentController(PaymentService paymentService,VnpayService vnpayService)
    {
        _paymentService = paymentService;
        _vnpayService = vnpayService;
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
        if (createPaymentDto.TotalAmount == 0)
        {
            // Tạo Enrollment trực tiếp mà không cần thanh toán
            var enrollmentDto = new CreateEnrollmentDto
            {
                UserId = createPaymentDto.UserId, // Giả sử CreatePaymentDto có UserId
                CourseId = createPaymentDto.CourseId, // Giả sử CreatePaymentDto có CourseId
                Status = "Active"
            };

            var enrollmentService = HttpContext.RequestServices.GetService<EnrollementService>();
            var existingEnrollment = await enrollmentService.GetByUserAndCourseAsync(enrollmentDto.UserId, enrollmentDto.CourseId.ToString());
            if (existingEnrollment != null)
            {
                return BadRequest("Bạn đã đăng ký khóa học này rồi");
            }

            var enrollment = await enrollmentService.CreateAsync(enrollmentDto);
            if (enrollment == null)
            {
                return StatusCode(500, "Lỗi khi tạo enrollment cho khóa học miễn phí.");
            }

            // Tạo PaymentDto với trạng thái "Paid" (vì miễn phí nên coi như đã thanh toán)
            var payment = new PaymentDto
            {
                Id = Guid.NewGuid(),
                UserId = createPaymentDto.UserId,
                CourseId = createPaymentDto.CourseId,
                Amount = 0,
                Status = "Paid",
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedAtAction(nameof(GetByIdAsync), new { id = payment.Id }, payment);
        }

        // Logic hiện tại cho khóa học có phí
        var paymentResult = await _paymentService.CreateAsync(createPaymentDto);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = paymentResult.Id }, paymentResult);
    }
    
    [HttpPost("create-payment-url")]
    [Authorize]
    public ActionResult<object> CreatePayment([FromBody] OrderDto orderDto)
    {
        if (orderDto == null || orderDto.Id == Guid.Empty || orderDto.TotalAmount <= 0)
        {
            return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
        }

        try
        {
            var paymentUrl = _vnpayService.CreatePaymentUrl(orderDto, HttpContext);
            Console.WriteLine("Generated Payment URL: " + paymentUrl); // Logging để debug
            return Ok(new { paymentUrl }); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating payment URL: {ex.Message}");
            return StatusCode(500, "Lỗi khi tạo URL thanh toán.");
        }
    }
    
    
    [HttpGet("payment-callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        var queryCollection = HttpContext.Request.Query;
        var result = await _vnpayService.PaymentExecuteAsync(queryCollection);
        return result;
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