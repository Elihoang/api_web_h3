using API_WebH3.DTO.Order;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class OrderService
{
     private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> CreateOrderWithDetailsAsync(CreateOrderDto orderDto)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = orderDto.UserId,
            Amount = orderDto.Amount,
            Status = orderDto.Status,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        var orderDetails = orderDto.OrderDetails.Select(detailDto => new OrderDetail
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            CourseId = detailDto.CourseId,
            Price = detailDto.Price,
            CouponId = detailDto.CouponId,
            DiscountAmount = detailDto.DiscountAmount,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        }).ToList();

        order.CourseId = orderDetails.FirstOrDefault()?.CourseId ?? Guid.Empty;

        await _orderRepository.CreateOrderAsync(order);

        foreach (var detail in orderDetails)
        {
            await _orderRepository.CreateOrderDetailsAsync(detail);
        }

        return await GetOrderById(order.Id);
    }

    public async Task<OrderDto> GetOrderById(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return null;
        }

        var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(id);

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CourseId = order.CourseId,
            Amount = order.Amount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            User = order.User,
            Course = order.Course,
            OrderDetails = orderDetails.ToList()
        };
    }

    public async Task UpdateOrderStatus(Guid orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        order.Status = status;
        await _orderRepository.UpdateAsync(order);
    }
}