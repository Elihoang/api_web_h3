using API_WebH3.DTOs.Enrollment;
using API_WebH3.DTOs.Order;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly VnpayService _vnpayService;
    private readonly OrderService _orderService;
    private readonly EnrollementService _enrollementService;

    public PaymentController(
        VnpayService vnpayService,
        OrderService orderService,
        EnrollementService enrollementService)
    {
        _vnpayService = vnpayService;
        _orderService = orderService;
        _enrollementService = enrollementService;
    }

    [HttpPost("create-payment-url")]
    [Authorize]
    public async Task<ActionResult<object>> CreatePayment([FromBody] OrderDto orderDto)
    {
        if (orderDto == null || orderDto.UserId == Guid.Empty || orderDto.CourseId == Guid.Empty || orderDto.Amount < 0)
        {
            Console.WriteLine("Invalid order data: " + (orderDto == null ? "orderDto is null" : orderDto.ToString()));
            return BadRequest("Dữ liệu đơn hàng không hợp lệ. Vui lòng kiểm tra UserId, CourseId và Amount.");
        }

        try
        {
            Console.WriteLine($"Received order data: UserId={orderDto.UserId}, CourseId={orderDto.CourseId}, Amount={orderDto.Amount}");

            // Giữ CreatedAt từ frontend nếu có, nếu không thì tạo mới
            if (string.IsNullOrEmpty(orderDto.CreatedAt))
            {
                orderDto.CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            }

            orderDto.Status = orderDto.Amount == 0 ? "Paid" : "Pending";

            if (orderDto.Id == Guid.Empty)
            {
                orderDto.Id = Guid.NewGuid();
            }
            Console.WriteLine($"Creating order: Id={orderDto.Id}, UserId={orderDto.UserId}, CourseId={orderDto.CourseId}, Amount={orderDto.Amount}, Status={orderDto.Status}, CreatedAt={orderDto.CreatedAt}");

            var createdOrder = await _orderService.CreateOrder(new CreateOrderDto
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                CourseId = orderDto.CourseId,
                Amount = orderDto.Amount,
                Status = orderDto.Status,
            });

            if (createdOrder == null)
            {
                Console.WriteLine("Failed to create order.");
                return StatusCode(500, "Không thể tạo đơn hàng. Vui lòng thử lại.");
            }

            if (orderDto.Amount == 0)
            {
                Console.WriteLine($"Processing free course for UserId={orderDto.UserId}, CourseId={orderDto.CourseId}");
                await _enrollementService.CreateAsync(new CreateEnrollmentDto
                {
                    UserId = orderDto.UserId,
                    CourseId = orderDto.CourseId,
                    Status = "Active"
                });
                return Ok(new { orderId = createdOrder.Id, isFree = true, message = "Đăng ký khóa học miễn phí thành công" });
            }

            var paymentUrl = _vnpayService.CreatePaymentUrl(createdOrder, HttpContext);
            if (string.IsNullOrEmpty(paymentUrl))
            {
                Console.WriteLine("Failed to generate payment URL.");
                return StatusCode(500, "Không thể tạo URL thanh toán. Vui lòng thử lại.");
            }

            Console.WriteLine("Generated Payment URL: " + paymentUrl);
            return Ok(new { paymentUrl, orderId = createdOrder.Id, isFree = false });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating payment URL: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi khi tạo URL thanh toán: {ex.Message}");
        }
    }

    [HttpGet("payment-callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        var queryCollection = HttpContext.Request.Query;
        Console.WriteLine("Payment callback received:");
        foreach (var (key, value) in queryCollection)
        {
            Console.WriteLine($"{key}: {value}");
        }

        var result = await _vnpayService.PaymentExecuteAsync(queryCollection);
        return result;
    }
}