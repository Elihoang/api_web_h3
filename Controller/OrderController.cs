using Microsoft.AspNetCore.Mvc;
using API_WebH3.DTO.Order;
using API_WebH3.Service;
using System;
using System.Threading.Tasks;
using API_WebH3.Helper;
using API_WebH3.Models;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ExcelExportHelper _excelExportHelper;

    public OrderController(OrderService orderService, ExcelExportHelper excelExportHelper)
    {
        _orderService = orderService;
        _excelExportHelper = excelExportHelper;
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

    [HttpGet("export-report")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string period = "all",
        [FromQuery] int? month = null,
        [FromQuery] int? year = null)
    {
        try
        {
            // Kiểm tra ngày hợp lệ
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            }

            // Kiểm tra tháng hợp lệ
            if (month.HasValue && (month < 1 || month > 12))
            {
                return BadRequest("Tháng phải nằm trong khoảng từ 1 đến 12.");
            }

            // Kiểm tra năm hợp lệ
            if (year.HasValue && (year < 1900 || year > DateTime.Now.Year))
            {
                return BadRequest($"Năm phải nằm trong khoảng từ 1900 đến {DateTime.Now.Year}.");
            }

            var fileBytes = await _excelExportHelper.GenerateOrderReportAsync(startDate, endDate, period, month, year);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return StatusCode(500, "Không thể tạo file báo cáo.");
            }

            string fileName = $"BaoCaoDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi khi xuất báo cáo: {ex.Message}");
        }
    }
}