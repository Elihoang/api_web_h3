using System;
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
        if (orderDto == null || orderDto.UserId == Guid.Empty || orderDto.Amount < 0 || !orderDto.OrderDetails.Any())
        {
            Console.WriteLine("Invalid order data: " + (orderDto == null ? "orderDto is null" : $"UserId={orderDto.UserId}, Amount={orderDto.Amount}, DetailsCount={orderDto.OrderDetails.Count}"));
            return BadRequest("Dữ liệu đơn hàng không hợp lệ. Vui lòng kiểm tra UserId, Amount và OrderDetails.");
        }

        try
        {
            Console.WriteLine($"Received order data: UserId={orderDto.UserId}, Amount={orderDto.Amount}, DetailsCount={orderDto.OrderDetails.Count}");

            orderDto.Status = orderDto.Amount == 0 ? "Paid" : "Pending";

            var createdOrder = await _orderService.CreateOrderWithDetailsAsync(orderDto);
            if (createdOrder == null)
            {
                Console.WriteLine("Failed to create order.");
                return StatusCode(500, "Không thể tạo đơn hàng. Vui lòng thử lại.");
            }

            if (orderDto.Amount == 0)
            {
                Console.WriteLine($"Processing free course for UserId={orderDto.UserId}");
                foreach (var detail in orderDto.OrderDetails)
                {
                    await _enrollementService.CreateAsync(new CreateEnrollmentDto
                    {
                        UserId = orderDto.UserId,
                        CourseId = detail.CourseId,
                        Status = "Active"
                    });
                }
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