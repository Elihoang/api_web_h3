using API_WebH3.DTOs.Order;
using API_WebH3.Models;
using API_WebH3.Repositories;

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
        var orderEntity = new Order
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = order.Status ?? "Pending",
        };

        try
        {
            await _orderRepository.CreateOrderAsync(orderEntity);
            Console.WriteLine($"Order created successfully: Id={orderEntity.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating order: {ex.Message}");
            throw new Exception("Không thể tạo đơn hàng: " + ex.Message);
        }

        var createdOrder = await _orderRepository.GetByIdAsync(orderEntity.Id);
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
            Console.WriteLine($"Order not found: Id={id}");
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
            Console.WriteLine($"No orders found for UserId={userId}");
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
            Console.WriteLine($"Order not found for status update: Id={id}");
            return null;
        }

        var orderEntity = new Order
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = status,
            CreatedAt = order.CreatedAt
        };

        try
        {
            await _orderRepository.UpdateAsync(orderEntity);
            Console.WriteLine($"Order status updated: Id={id}, Status={status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
            throw new Exception("Không thể cập nhật trạng thái đơn hàng: " + ex.Message);
        }

        var updatedOrder = await _orderRepository.GetByIdAsync(id);
        return new OrderDto
        {
            Id = updatedOrder.Id,
            UserId = updatedOrder.UserId,
            UserName = updatedOrder.User?.FullName,
            CourseId = updatedOrder.CourseId,
            CourseName = updatedOrder.Course?.Title,
            Amount = updatedOrder.Amount,
            Status = updatedOrder.Status,
            CreatedAt = updatedOrder.CreatedAt
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