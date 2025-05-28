using Microsoft.AspNetCore.Mvc;
using API_WebH3.DTO.Order;
using API_WebH3.Service;
using System;
using System.Threading.Tasks;
using API_WebH3.Models;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetAllOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        var (pagedOrders, totalItems) = await _orderService.GetAllOrdersAsync(pageNumber, pageSize);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var result = new
        {
            Data = pagedOrders,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(string id)
    {
        var orderDto = await _orderService.GetOrderById(id);
        if (orderDto == null)
        {
            return NotFound();
        }

        return Ok(orderDto);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId(Guid userId)
    {
        var orderDtos = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orderDtos);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var orderDto = await _orderService.CreateOrderWithDetailsAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(string id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _orderService.UpdateOrderStatus(id, updateOrderDto.Status);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("{orderId}/details")]
    public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails(string orderId)
    {
        var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
        if (!orderDetails.Any())
        {
            return NotFound();
        }

        return Ok(orderDetails);
    }
}