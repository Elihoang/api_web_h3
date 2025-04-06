using API_WebH3.DTOs.Order;
using API_WebH3.Models;
using API_WebH3.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> CreateOrder(CreateOrderDto order)
    {
        // Tạo đơn hàng
        var orders = new CreateOrderDto
        {
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = "Pending"
        };
        await _orderRepository.CreateOrderAsync(orders);
        var orderDto = new OrderDto
        {
            Id = orders.Id,
            UserId = orders.UserId,
            CourseId = orders.CourseId,
            Amount = orders.Amount,
            Status = orders.Status,
        };
        return orderDto;
    }

    public async Task<OrderDto> GetOrderById(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        
        if (order == null)
        {
            return null;
        }
        
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }
    
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(Guid userId)
    {
        // Bạn cần thêm phương thức này vào IOrderRepository và OrderRepository
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        
        if (orders == null || !orders.Any())
        {
            return Enumerable.Empty<OrderDto>();
        }
        
        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        });
    }
    
    public async Task<OrderDto> UpdateOrderStatus(Guid id, string status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        
        if (order == null)
        {
            return null;
        }
        // Cập nhật trạng thái đơn hàng


        order.Status = status;
        await _orderRepository.UpdateAsync(order);
        
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }
}