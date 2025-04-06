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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllAsync()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _orderService.CreateOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi khi tạo đơn hàng: {ex.Message}");
        }
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

    [HttpGet("ユーザー/{userId}")]
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
        try
        {
            var result = await _orderService.UpdateOrderStatus(id, status);
            if (result == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với ID: {id}");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
        }
    }
}