using System;
using System.Text.Json;
using System.Threading.Tasks;
using API_WebH3.DTO.Enrollment;
using API_WebH3.DTO.Order;
using API_WebH3.Service;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;

[Route("api/payment/momo")]
[ApiController]
public class PaymentMomoController : ControllerBase
{
    private readonly MomoService _momoService;
    private readonly OrderService _orderService;
    private readonly EnrollmentService _enrollmentService;

    public PaymentMomoController(
        MomoService momoService,
        OrderService orderService,
        EnrollmentService enrollmentService)
    {
        _momoService = momoService;
        _orderService = orderService;
        _enrollmentService = enrollmentService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<object>> CreatePayment([FromBody] CreateOrderDto orderDto)
    {
        Console.WriteLine($"Nhận yêu cầu MOMO: {JsonSerializer.Serialize(orderDto)}");

        try
        {
            if (orderDto == null || orderDto.UserId == Guid.Empty || !orderDto.OrderDetails.Any())
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");

            orderDto.Status = orderDto.Amount == 0 ? "Paid" : "Pending";
            var createdOrder = await _orderService.CreateOrderWithDetailsAsync(orderDto);

            if (createdOrder == null)
                return StatusCode(500, "Không thể tạo đơn hàng.");

            if (orderDto.Amount == 0)
            {
                foreach (var detail in orderDto.OrderDetails)
                {
                    await _enrollmentService.CreateAsync(new CreateEnrollmentDto
                    {
                        UserId = orderDto.UserId,
                        CourseId = detail.CourseId,
                        Status = "Enrolled"
                    });
                }

                return Ok(new { orderId = createdOrder.Id, isFree = true, message = "Đăng ký thành công" });
            }

            var paymentUrl = await _momoService.CreatePaymentUrlAsync(createdOrder);
            if (string.IsNullOrEmpty(paymentUrl))
                return StatusCode(500, "Không thể tạo URL thanh toán.");

            return Ok(new { paymentUrl, orderId = createdOrder.Id, isFree = false });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi trong CreatePayment (Momo): {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        return await _momoService.PaymentCallbackAsync(Request.Query);
    }
}
