using API_WebH3.DTOs.Order;
using API_WebH3.Services;
using API_WebH3.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = new Order
        {
            UserId = request.UserId,
            TotalAmount = request.OrderDetails.Sum(d => d.Price),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var result = await _orderService.CreateOrder(order, request.OrderDetails);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _orderService.GetOrderById(id);
        
        if (order == null)
        {
            return NotFound($"Không tìm thấy đơn hàng với ID: {id}");
        }

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(Guid userId)
    {
        var orders = await _orderService.GetOrdersByUserId(userId);
        
        if (orders == null || !orders.Any())
        {
            return NotFound($"Không tìm thấy đơn hàng nào cho người dùng với ID: {userId}");
        }

        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string status)
    {
        var result = await _orderService.UpdateOrderStatus(id, status);
        
        if (result == null)
        {
            return NotFound($"Không tìm thấy đơn hàng với ID: {id}");
        }

        return Ok(result);
    }
    
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        var details = await _orderService.GetOrderDetailsById(id);
    
        if (details == null || !details.Any())
        {
            return NotFound($"Không tìm thấy chi tiết đơn hàng với ID: {id}");
        }

        return Ok(details);
    }
}