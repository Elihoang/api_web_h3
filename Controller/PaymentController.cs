using System;
using System.Text.Json;
using System.Threading.Tasks;
using API_WebH3.DTO.Enrollment;
using API_WebH3.DTO.Order;
using API_WebH3.Service;
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
    private readonly EnrollmentService _enrollementService;

    public PaymentController(
        VnpayService vnpayService,
        OrderService orderService,
        EnrollmentService enrollementService)
    {
        _vnpayService = vnpayService;
        _orderService = orderService;
        _enrollementService = enrollementService;
    }

    [HttpPost("create-payment-url")]
    public async Task<ActionResult<object>> CreatePayment([FromBody] CreateOrderDto orderDto)
    {
        Console.WriteLine($"Nhận yêu cầu: {JsonSerializer.Serialize(orderDto)}");
        try
        {
            if (orderDto == null || orderDto.UserId == Guid.Empty || !orderDto.OrderDetails.Any())
            {
                Console.WriteLine("Dữ liệu đơn hàng không hợp lệ.");
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            Console.WriteLine("Đặt trạng thái đơn hàng...");
            orderDto.Status = orderDto.Amount == 0 ? "Paid" : "Pending";

            Console.WriteLine("Tạo đơn hàng...");
            var createdOrder = await _orderService.CreateOrderWithDetailsAsync(orderDto);
            if (createdOrder == null)
            {
                Console.WriteLine("Không thể tạo đơn hàng.");
                return StatusCode(500, "Không thể tạo đơn hàng.");
            }

            if (orderDto.Amount == 0)
            {
                Console.WriteLine("Xử lý khóa học miễn phí...");
                foreach (var detail in orderDto.OrderDetails)
                {
                    await _enrollementService.CreateAsync(new CreateEnrollmentDto
                    {
                        UserId = orderDto.UserId,
                        CourseId = detail.CourseId,
                        Status = "Enrolled"
                    });
                }
                return Ok(new { orderId = createdOrder.Id, isFree = true, message = "Đăng ký thành công" });
            }

            Console.WriteLine("Tạo URL thanh toán...");
            var paymentUrl = _vnpayService.CreatePaymentUrl(createdOrder, HttpContext);
            if (string.IsNullOrEmpty(paymentUrl))
            {
                Console.WriteLine("Không thể tạo URL thanh toán.");
                return StatusCode(500, "Không thể tạo URL thanh toán.");
            }

            Console.WriteLine($"URL thanh toán: {paymentUrl}");
            return Ok(new { paymentUrl, orderId = createdOrder.Id, isFree = false });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi trong CreatePayment: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
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