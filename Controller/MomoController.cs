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
        Console.WriteLine($"Received MOMO request: {JsonSerializer.Serialize(orderDto)}");

        try
        {
            if (orderDto == null || orderDto.UserId == Guid.Empty || !orderDto.OrderDetails.Any())
                return BadRequest("Invalid order data.");

            orderDto.Status = orderDto.Amount == 0 ? "Paid" : "Pending";
            var createdOrder = await _orderService.CreateOrderWithDetailsAsync(orderDto);

            if (createdOrder == null)
                return StatusCode(500, "Failed to create order.");

            if (orderDto.Amount == 0)
            {
                foreach (var detail in orderDto.OrderDetails)
                {
                    var existingEnrollment = await _enrollmentService.GetByUserAndCourseAsync(orderDto.UserId, detail.CourseId);
                    if (existingEnrollment == null)
                    {
                        await _enrollmentService.CreateAsync(new CreateEnrollmentDto
                        {
                            UserId = orderDto.UserId,
                            CourseId = detail.CourseId,
                            Status = "Enrolled"
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Enrollment already exists for UserId: {orderDto.UserId}, CourseId: {detail.CourseId}");
                    }
                }

                return Ok(new { orderId = createdOrder.Id, isFree = true, message = "Registration successful" });
            }

            var paymentUrl = await _momoService.CreatePaymentUrlAsync(createdOrder);
            if (string.IsNullOrEmpty(paymentUrl))
                return StatusCode(500, "Failed to create payment URL.");

            return Ok(new { paymentUrl, orderId = createdOrder.Id, isFree = false });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreatePayment (Momo): {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        return await _momoService.PaymentCallbackAsync(Request.Query);
    }
}
