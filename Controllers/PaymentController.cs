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

    public PaymentController(VnpayService vnpayService)
    {
        _vnpayService = vnpayService;
    }

    [HttpPost("create-payment-url")]
    [Authorize]
    public ActionResult<object> CreatePayment([FromBody] OrderDto orderDto)
    {
        if (orderDto == null || orderDto.Id == Guid.Empty || orderDto.Amount <= 0)
        {
            return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
        }

        try
        {
            var paymentUrl = _vnpayService.CreatePaymentUrl(orderDto, HttpContext);
            Console.WriteLine("Generated Payment URL: " + paymentUrl);
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
}