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
        await _orderRepository.CreateOrderAsync(order);
        var createdOrder = await _orderRepository.GetByIdAsync(order.Id); // Lấy đơn hàng vừa tạo
        if (createdOrder == null)
        {
            throw new Exception("Không thể tìm thấy đơn hàng vừa tạo.");
        }
        return new OrderDto
        {
            Id = createdOrder.Id,
            UserId = createdOrder.UserId,
            UserName = createdOrder.User?.FullName,
            CourseId = createdOrder.CourseId,
            CourseName = createdOrder.Course?.Title,
            Amount = createdOrder.Amount,
            Status = createdOrder.Status,
            CreatedAt = createdOrder.CreatedAt
        };
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
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        if (orders == null || !orders.Any())
        {
            return Enumerable.Empty<OrderDto>();
        }
        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
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
        order.Status = status;
        await _orderRepository.UpdateAsync(order);
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            CourseId = order.CourseId,
            CourseName = order.Course?.Title,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        });
    }
}