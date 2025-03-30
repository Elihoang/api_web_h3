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

    public async Task<OrderDto> CreateOrder(Order order, List<OrderDetailsDto> detailsDto = null)
    {
        // Tạo đơn hàng
        var orders = new Order
        {
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };
        await _orderRepository.CreateOrderAsync(orders);
    
        var orderDto = new OrderDto
        {
            Id = orders.Id,
            UserId = orders.UserId,
            TotalAmount = orders.TotalAmount,
            Status = orders.Status,
            CreatedAt = orders.CreatedAt,
            OrderDetails = new List<OrderDetailsDto>()
        };
    
        // Nếu có chi tiết đơn hàng
        if (detailsDto != null && detailsDto.Any())
        {
            foreach (var detail in detailsDto)
            {
                var orderDetails = new OrderDetail
                {
                    OrderId = orders.Id,
                    CourseId = detail.CourseId,
                    Price = detail.Price,
                    CreatedAt = DateTime.UtcNow
                };
                await _orderRepository.CreateOrderDetailsAsync(orderDetails);
            
                // Thêm vào DTO để trả về
                orderDto.OrderDetails.Add(new OrderDetailsDto
                {
                    Id = orderDetails.Id,
                    OrderId = orderDetails.OrderId,
                    CourseId = orderDetails.CourseId,
                    Price = orderDetails.Price,
                    CreatedAt = orderDetails.CreatedAt
                });
            }
        }

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
            TotalAmount = order.TotalAmount,
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
            TotalAmount = order.TotalAmount,
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
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }
    public async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsById(Guid orderId)
    {
        var details = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
    
        return details.Select(d => new OrderDetailsDto
        {
            Id = d.Id,
            OrderId = d.OrderId,
            CourseId = d.CourseId,
            Price = d.Price,
            CreatedAt = d.CreatedAt
        });
    }
}