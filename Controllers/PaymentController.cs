using API_WebH3.DTOs.Order;
using API_WebH3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PaymentController : Controller
{
    private readonly VnpayService _vnPayService;

    public PaymentController(VnpayService vnPayService)
    {
        _vnPayService = vnPayService;
    }

    [HttpPost("create-payment-url")]
    public IActionResult CreatePaymentUrl([FromBody] OrderDto orderDto)
    {
        try
        {
            var url = _vnPayService.CreatePaymentUrl(orderDto, HttpContext);

            // In ra log ?? ki?m tra
            Console.WriteLine("Generated Payment URL: " + url);

            return Ok(new { PaymentUrl = url });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating payment URL: " + ex.Message);
            return BadRequest(new { Error = "Failed to create payment URL", Details = ex.Message });
        }
    }


    [HttpGet("payment-callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        var response = await _vnPayService.PaymentExecuteAsync(Request.Query);
        return Ok(response);
    }
}